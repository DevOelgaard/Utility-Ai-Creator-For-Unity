using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

internal class PersistenceAPI
{
    public IPersister Persister { get; private set; }
    // private readonly IPersister destructivePersister;

    internal static PersistenceAPI Instance => _instance ??= new PersistenceAPI(new JsonPersister());
    private static PersistenceAPI _instance;

    private PersistenceAPI(IPersister persister)
    {
        this.Persister = persister;
        // this.destructivePersister = new JsonDestructivePersister();
    }

    internal void SetPersister(IPersister p)
    {
        this.Persister = p;
    }

    internal async Task SaveObjectPanel(RestoreAble o)
    {
        var extension = FileExtensionService.GetExtension(o);
        var path = EditorUtility.SaveFilePanel("Save object", "", "name", extension);
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        await SaveObjectPath(o, Path.GetDirectoryName(path) + @"\", Path.GetFileName(path));
    }

    internal async Task SaveObjectsPanel(List<RestoreAble> restoreables)
    {
        var path = EditorUtility.SaveFolderPanel("Save object", "folder", "default name");
        foreach (var r in restoreables)
        {
            await SaveObjectPath(r, path, r.FileName);
        }
    }

    internal async Task SaveObjectPath(RestoreAble o, string path, string fileName)
    {
        await o.SaveToFile(path, Persister, -2, fileName);
    }

    internal async Task SaveDestructiveObjectPathAsync(RestoreAble o, string path, string fileName)
    {
        var startTime = DateTime.Now;
        await o.SaveToFile(path, Persister,-2, fileName);
        DebugService.Log("Done saving destructively path: " + path, this);
        await CleanUpAsync(path, startTime);
    }
    
    
    internal void SaveDestructiveObjectPath(RestoreState o, string path)
    {
        DebugService.Log("Saving destructively path: " + path, this);

        var startTime = DateTime.Now;
        Persister.SaveObject(o,path);
        DebugService.Log("Saving destructively Complete path: " + path, this);
        CleanUp(path, startTime);
    }

    internal async Task<ObjectMetaData<T>> LoadObjectPanel<T>()
    {
        var extension = FileExtensionService.GetExtension(typeof(T));
        var path = EditorUtility.OpenFilePanel("Load object", "", extension);
        var o = await Persister.LoadObjectAsync<T>(path);
        return o;
    }

    internal async Task<ObjectMetaData<T>> LoadObjectPanel<T>(string[] filters)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Load object", "", filters);
        var o = await Persister.LoadObjectAsync<T>(path);
        return o;
    }
    
    internal ObjectMetaData<T> LoadObjectPath<T>(string path)
    {
        var result = Persister.LoadObject<T>(path);
        return result;
    }

    internal async Task<ObjectMetaData<T>> LoadObjectPathAsync<T>(string path)
    {
        var result = await Persister.LoadObjectAsync<T>(path);
        return result;
    }

    internal async Task<List<ObjectMetaData<T>>> LoadObjectsPanel<T>(string startPath, string filter = "") where T : RestoreState
    {
        var path = EditorUtility.OpenFolderPanel("Load object", startPath, "default name");
        var res = await Persister.LoadObjects<T>(path, "*"+filter);
        return res;
    }

    internal async Task<List<ObjectMetaData<T>>> LoadObjectsPath<T>(string folderPath, string filter = "") where T: RestoreState
    {
        var results = await Persister.LoadObjects<T>(folderPath, "*"+filter);
        foreach (var result in results
                     .Where(result => result.LoadedObject != null))
        {
            result.LoadedObject.FolderLocation = folderPath;
        }

        return results;
    }

    internal async Task<List<ObjectMetaData<T>>> LoadObjectsPathWithFilters<T>(string folderPath, Type t) where T : RestoreState
    {
        var filter = FileExtensionService.GetFileExtensionFromType(t);
        var res = await LoadObjectsPath<T>(folderPath, filter);
        return res;
    }

    internal async Task<List<ObjectMetaData<T>>> LoadObjectsPathWithFiltersAndSubDirectories<T>(string folderPath, Type t) where T : RestoreState
    {
        var filter = FileExtensionService.GetFileExtensionFromType(t);
        var result = await LoadObjectsPath<T>(folderPath, filter);
        try
        {
            var subDirectories = Directory.GetDirectories(folderPath);
            foreach (var subDirectory in subDirectories)
            {
                result = await  LoadObjectsPath<T>(subDirectory, filter);
            }

        } catch (Exception ex)
        {
            if (ex.GetType() == typeof(DirectoryNotFoundException))
            {
                return result;
            }
            else
            {
                throw;
            }
        }

        return result;
    }

    internal async Task<ObjectMetaData<T>> LoadFilePanel<T>(string[] filters)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Load object", "", filters);
        var res = await LoadFilePath<T>(path);
        return res;
    }

    private async Task<ObjectMetaData<T>> LoadFilePath<T>(string path)
    {
        var res = await Persister.LoadObjectAsync<T>(path);
        return res;
    }

    private static void CleanUp(string path, DateTime startTime)
    {
        DebugService.Log("Starting cleanup path: " + path, nameof(PersistenceAPI));
        var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (var file in files.Where(f => !f.Contains(".meta")))
        {
            var lastWriteTime = File.GetLastWriteTime(file);
            if (lastWriteTime < startTime)
            {
                File.Delete(file);
            }
        }
        DeleteEmptyFolders(path);
    }
    
    private static async Task CleanUpAsync(string path, DateTime startTime)
    {
        DebugService.Log("Starting cleanup path: " + path, nameof(PersistenceAPI));
        var t = Task.Factory.StartNew(() =>
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files.Where(f => !f.Contains(".meta")))
            {
                var lastWriteTime = File.GetLastWriteTime(file);
                if (lastWriteTime < startTime)
                {
                    File.Delete(file);
                }
            }
        });
        await t;
        await DeleteEmptyFoldersAsync(path);
    }

    // https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
    private static async Task DeleteEmptyFoldersAsync(string path)
    {
         if (path.Contains("."))
         {
             path = new DirectoryInfo(Path.GetDirectoryName(path) ?? string.Empty).FullName;
         }
         var directories = Directory.GetDirectories(path);
         var tasks = new List<Task>();
         
         foreach (var d in directories.Where((d => !d.Contains("."))))
         {
             await DeleteEmptyFoldersAsync(d);
             tasks.Add(Task.Factory.StartNew(() =>
             {
                 var metaFiles = Directory.GetFiles(d).Where(f => f.Contains(".meta"));
                 var fileNamesWithoutMeta = Directory.GetFiles(d)
                     .Where(f => !f.Contains(".meta"))
                     .Select(Path.GetFileName)
                     .ToList();
                 var childDirectoryNames = Directory.GetDirectories(d)
                     .Select(Path.GetFileNameWithoutExtension)
                     .ToList();
                 
                 foreach (var metaFile in metaFiles)
                 {
                     var metaFileName = Path.GetFileNameWithoutExtension(metaFile);
                     var canDelete = fileNamesWithoutMeta.All(file => file != metaFileName) &&
                                     childDirectoryNames.All(directory => directory != metaFileName);
        
                     if (!canDelete) continue;
                     File.Delete(metaFile);
                 }
        
                 // Delete empty folders
                 var childDirectories = Directory.GetDirectories(d);
                 if (childDirectories.Length > 0)
                 {
                     return;
                 }
        
                 var filesWithoutMeta = Directory.GetFiles(d)
                     .Where(f => !f.Contains(".meta"))
                     .ToList();
             
                 if (filesWithoutMeta.Count > 0)
                 {
                     return;
                 }
                 foreach (var file in Directory.GetFiles(d))
                 {
                     File.Delete(file);
                 }
        
                 Directory.Delete(d,false);
             }));
         }
        
         await Task.WhenAll(tasks);
    }
    
        // https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
    private static void DeleteEmptyFolders(string path)
    {
         if (path.Contains("."))
         {
             path = new DirectoryInfo(Path.GetDirectoryName(path) ?? string.Empty).FullName;
         }
         var directories = Directory.GetDirectories(path);
         
         foreach (var d in directories.Where((d => !d.Contains("."))))
         {
             DeleteEmptyFolders(d);
             var metaFiles = Directory.GetFiles(d).Where(f => f.Contains(".meta"));
             var fileNamesWithoutMeta = Directory.GetFiles(d)
                 .Where(f => !f.Contains(".meta"))
                 .Select(Path.GetFileName)
                 .ToList();
             var childDirectoryNames = Directory.GetDirectories(d)
                 .Select(Path.GetFileNameWithoutExtension)
                 .ToList();
                 
             foreach (var metaFile in metaFiles)
             {
                 var metaFileName = Path.GetFileNameWithoutExtension(metaFile);
                 var canDelete = fileNamesWithoutMeta.All(file => file != metaFileName) &&
                                 childDirectoryNames.All(directory => directory != metaFileName);
        
                 if (!canDelete) continue;
                 File.Delete(metaFile);
             }
        
             // Delete empty folders
             var childDirectories = Directory.GetDirectories(d);
             if (childDirectories.Length > 0)
             {
                 return;
             }
        
             var filesWithoutMeta = Directory.GetFiles(d)
                 .Where(f => !f.Contains(".meta"))
                 .ToList();
             
             if (filesWithoutMeta.Count > 0)
             {
                 return;
             }
             foreach (var file in Directory.GetFiles(d))
             {
                 File.Delete(file);
             }
        
             Directory.Delete(d,false);
         }
    }
}

public class ObjectMetaData<T>
{
    public bool Success = true;
    public readonly Type StateType;
    public readonly Type ModelType;
    public readonly T LoadedObject;
    public readonly Type type;
    public readonly string Path;
    public string ErrorMessage;
    public Exception Exception;

    public ObjectMetaData(T o, string path)
    {
        StateType = FileExtensionService.GetStateFromFileName(path);
        ModelType = FileExtensionService.GetTypeFromFileName(path);
        type = typeof(T);
        LoadedObject = o;
        Path = path;
    }
}