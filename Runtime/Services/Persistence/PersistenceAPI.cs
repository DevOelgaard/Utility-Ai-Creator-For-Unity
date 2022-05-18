using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Codice.Client.BaseCommands.Changelist;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

internal class PersistenceAPI
{
    public IPersister Persister { get; set; }

    internal static PersistenceAPI Instance => _instance ??= new PersistenceAPI(new JsonPersister());
    private static PersistenceAPI _instance;

    private PersistenceAPI(IPersister persister)
    {
        Persister = persister;
        // this.destructivePersister = new JsonDestructivePersister();
    }

    #region Save

    internal async Task SaveObjectPanelAsync(RestoreAble o)
    {
        var extension = FileExtensionService.GetFileExtensionFromType(o.GetType());
        var path = EditorUtility.SaveFilePanel("Save object", "", "name", extension);
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        await SaveObjectAsync(o, Path.GetDirectoryName(path));
    }

    internal async Task SaveObjectsPanelAsync(List<RestoreAble> restoreables)
    {
        var path = EditorUtility.SaveFolderPanel("Save object", "folder", "default name");
        foreach (var r in restoreables)
        {
            await SaveObjectAsync(r, path);
        }
    }



    internal async Task SaveObjectDestructivelyAsync(RestoreAble o, string path)
    {
        var startTime = DateTime.Now;
        await SaveObjectAsync(o,path);
        DebugService.Log("Done saving destructively path: " + path, this);
        await CleanUpAsync(path, startTime);
    }
    
    internal async Task SaveObjectAsync(RestoreAble o, string path)
    {
        if (path.Length > Consts.MaxPathLengthWindows)
        {
            var message = "The path you are trying to save is: " + path.Length +
                          " chars long. The max allowed path length is: " +
                          Consts.MaxPathLengthWindows;
            DebugService.LogError(message,this);
            throw new ConstraintException(message);
        }
        var task = o.SaveToFile(path, Persister, -2);
        if (await Task.WhenAny(task, Task.Delay(Settings.TimeOutMs)) == task)
        {
            return;
        }
        else
        {
            throw new TimeoutException("SaveObjectAsync timed out after: " +
                                       Settings.TimeOutMs + "ms. " + " FileName: " + o.FileName + " Path: " + path);
        }
    }

    #endregion

    #region Load

    internal async Task<ObjectMetaData<T>> LoadObjectPanel<T>()
    {
        var extension = FileExtensionService.GetFileExtensionFromType(typeof(T));
        var path = EditorUtility.OpenFilePanel("Load object", "", extension);
        var o = await LoadObjectPathAsync<T>(path);
        return o;
    }

    internal async Task<ObjectMetaData<T>> LoadObjectPanel<T>(string[] filters)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Load object", "", filters);
        var o = await LoadObjectPathAsync<T>(path);
        return o;
    }
    
    internal ObjectMetaData<T> LoadObjectPath<T>(string path)
    {
        var result = Persister.LoadObject<T>(path);
        return result;
    }

    internal async Task<ObjectMetaData<T>> LoadObjectPathAsync<T>(string path)
    {
        DebugService.Log("Loading object of type: " + typeof(T) + " at path: " + path, this );
        var task = Persister.LoadObjectAsync<T>(path);
        if (await Task.WhenAny(task, Task.Delay(Settings.TimeOutMs)) == task)
        {
            return task.Result;
        }
        else
        {
            throw new TimeoutException("LoadObjectPathAsync timed out after: " +
                                       Settings.TimeOutMs + "ms. " + " Path: " + path);
        }
    }

    // internal async Task<List<ObjectMetaData<T>>> LoadObjectsPanel<T>(string startPath, string filter = "") where T : RestoreState
    // {
    //     var path = EditorUtility.OpenFolderPanel("Load object", startPath, "default name");
    //     var res = await LoadObjectsPathAsync<T>(path, "*"+filter);
    //     return res;
    // }



    internal async Task<List<ObjectMetaData<T>>> LoadObjectsOfTypeAsync<T>(string folderPath, Type t) where T : RestoreState
    {
        var filter = FileExtensionService.GetFileExtensionFromType(t);
        var res = await LoadObjectsAsync<T>(folderPath, filter);
        return res;
    }
    
    internal async Task<List<ObjectMetaData<T>>> LoadObjectsAsync<T>(string folderPath, string filter = "") where T: RestoreState
    {
        var task = Persister.LoadObjectsAsync<T>(folderPath, "*"+filter);
        if (await Task.WhenAny(task, Task.Delay(Settings.TimeOutMs)) == task)
        {
            DebugService.Log("Loaded objects count: " + task.Result.Count + " path: " + folderPath, this);
            return task.Result;
        }
        else
        {
            throw new TimeoutException("LoadObjectsPathAsync timed out after: " +
                                       Settings.TimeOutMs + "ms. " + " Filter: " + filter + " FolderPath: " + folderPath);
        }
    }

    internal async Task<ObjectMetaData<T>> LoadFilePanel<T>(string[] filters)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Load object", "", filters);
        var res = await LoadObjectPathAsync<T>(path);
        return res;
    }

    #endregion

    #region Div

    private static void CleanUp(string path, DateTime startTime)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (directoryInfo.Name == "Assets")
        {
            DebugService.LogError("Trying to edit Assets folder. Returning path: " + path, nameof(PersistenceAPI));
            return;
        }
            
        DebugService.Log("Clean up Start path: " + path, nameof(PersistenceAPI));
        if (path.Contains("."))
        {
            DebugService.Log("Only intended or directories returning : " + path, nameof(PersistenceAPI));
            return;
        }
        var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
        foreach (var file in files.Where(f => !f.Contains(".meta")))
        {
            var lastWriteTime = File.GetLastWriteTime(file);
            if (lastWriteTime < startTime)
            {
                DeleteFileIfAllowed(file);
            }
        }
        DeleteEmptyFolders(path);
        DebugService.Log("Clean up Complete path: " + path, nameof(PersistenceAPI));

    }
    
    internal static async Task CleanUpAsync(string path, DateTime startTime)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (directoryInfo.Name == "Assets")
        {
            DebugService.LogError("Trying to edit Assets folder. Returning path: " + path, nameof(PersistenceAPI));
            return;
        }
        DebugService.Log("Clean up Start path: " + path, nameof(PersistenceAPI));
        if (!Directory.Exists(path)) return;
        var t = Task.Factory.StartNew(() =>
        {
            var files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (var file in files.Where(f => !f.Contains(".meta")))
            {
                var lastWriteTime = File.GetLastWriteTime(file);
                if (lastWriteTime < startTime)
                {
                    if (!FileIsUasFile(file))
                    {
                        continue;
                    }
                    DeleteFileIfAllowed(file);
                }
            }
        });
        await t;
        await DeleteEmptyFoldersAsync(path);
        DebugService.Log("Clean up Complete path: " + path, nameof(PersistenceAPI));

    }

    // https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
    private static async Task DeleteEmptyFoldersAsync(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (directoryInfo.Name == "Assets")
        {
            DebugService.LogError("Trying to edit Assets folder. Returning path: " + path, nameof(PersistenceAPI));
            return;
        }
        DebugService.Log("Deleting Folders at path: " + path, nameof(PersistenceAPI));
         if (path.Contains("."))
         {
             path = new DirectoryInfo(Path.GetDirectoryName(path) ?? string.Empty).FullName;
         }
         DebugService.Log("Getting directories at path: " + path, nameof(PersistenceAPI));

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
                     DeleteFileIfAllowed(metaFile);
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
                     DeleteFileIfAllowed(file);

                 }
        
                 Directory.Delete(d,false);
             }));
         }
        
         await Task.WhenAll(tasks);
         DebugService.Log("Deleting Folders Complete at path: " + path, nameof(PersistenceAPI));

    }
    
        // https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
    private static void DeleteEmptyFolders(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        if (directoryInfo.Name == "Assets")
        {
            DebugService.LogError("Trying to edit Assets folder. Returning path: " + path, nameof(PersistenceAPI));
            return;
        }
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
                 
            // Clean up meta files
            foreach (var metaFile in metaFiles)
            {
                var metaFileName = Path.GetFileNameWithoutExtension(metaFile);
                var canDelete = fileNamesWithoutMeta.All(file => file != metaFileName) &&
                                childDirectoryNames.All(directory => directory != metaFileName);
        
                if (!canDelete) continue;
                DeleteFileIfAllowed(metaFile);
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
                DeleteFileIfAllowed(file);
            }
        
            Directory.Delete(d,false);
        }

    }

    private static void DeleteFileIfAllowed(string file)
    {
        if (FileIsUasFile(file))
        {
            File.Delete(file);
        }
    }

    private static bool FileIsUasFile(string file)
    {
        foreach (var fileExtension in Consts.FileExtensions)
        {
            if (file.Contains("."+fileExtension))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}

public class ObjectMetaData<T>
{
    public bool IsSuccessFullyLoaded = true;
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
        if (LoadedObject == null)
        {
            IsSuccessFullyLoaded = false;
        }
    }
}
