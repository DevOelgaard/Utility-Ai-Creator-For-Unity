using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
internal class PersistenceAPI
{
    private IPersister persister;
    private readonly IPersister destructivePersister;

    internal static PersistenceAPI Instance => _instance ??= new PersistenceAPI(new JsonPersister());
    private static PersistenceAPI _instance;

    private PersistenceAPI(IPersister persister)
    {
        this.persister = persister;
        this.destructivePersister = new JsonDestructivePersister();
    }

    internal void SetPersister(IPersister p)
    {
        this.persister = p;
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
        await o.SaveToFile(path, persister, -2, fileName);
    }

    internal async Task SaveDestructiveObjectPath(RestoreAble o, string path, string fileName)
    {
        await o.SaveToFile(path, destructivePersister,-2, fileName);
        await CleanUp(path);
    }

    internal async Task<ObjectMetaData<T>> LoadObjectPanel<T>()
    {
        var extension = FileExtensionService.GetExtension(typeof(T));
        var path = EditorUtility.OpenFilePanel("Load object", "", extension);
        var o = await persister.LoadObject<T>(path);
        return o;
    }

    internal async Task<ObjectMetaData<T>> LoadObjectPanel<T>(string[] filters)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Load object", "", filters);
        var o = await persister.LoadObject<T>(path);
        return o;
    }

    internal async Task<ObjectMetaData<T>> LoadObjectPath<T>(string path)
    {
        var result = await persister.LoadObject<T>(path);
        // CleanUp(path);
        return result;
    }

    internal async Task<List<ObjectMetaData<T>>> LoadObjectsPanel<T>(string startPath, string filter = "") where T : RestoreState
    {
        var path = EditorUtility.OpenFolderPanel("Load object", startPath, "default name");
        return await persister.LoadObjects<T>(path, "*"+filter);
    }

    internal async Task<List<ObjectMetaData<T>>> LoadObjectsPath<T>(string folderPath, string filter = "") where T: RestoreState
    {
        var results = await persister.LoadObjects<T>(folderPath, "*"+filter);
        foreach (var result in results)
        {
            if (result.LoadedObject == null) continue;
            result.LoadedObject.FolderLocation = folderPath;
        }

        return results;
    }

    internal async Task<List<ObjectMetaData<T>>> LoadObjectsPathWithFilters<T>(string folderPath, Type t) where T : RestoreState
    {
        var filter = FileExtensionService.GetFileExtensionFromType(t);
        return await LoadObjectsPath<T>(folderPath, filter);
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
            // result
            //     .AddRange(subDirectories
            //         .SelectMany(subDirectory => LoadObjectsPath<T>(subDirectory, filter)));
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
        return await LoadFilePath<T>(path);
    }

    private async Task<ObjectMetaData<T>> LoadFilePath<T>(string path)
    {
        return await persister.LoadObject<T>(path);
    }

    //https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
    private async Task CleanUp(string path)
    {
        if (path.Contains("."))
        {
            path = new DirectoryInfo(System.IO.Path.GetDirectoryName(path) ?? string.Empty).FullName;
        }
        var directories = Directory.GetDirectories(path);
        var tasks = new List<Task>();
        foreach (var d in directories.Where((d => !d.Contains("."))))
        {
            await CleanUp(d);
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
                if (childDirectories.Length > 0) return;

                var filesWithoutMeta = Directory.GetFiles(d)
                    .Where(f => !f.Contains(".meta"))
                    .ToList();
            
                if (filesWithoutMeta.Count > 0) return;
                foreach (var file in Directory.GetFiles(d))
                {
                    File.Delete(file);
                }

                Directory.Delete(d,false);
            }));
        }

        await Task.WhenAll(tasks);
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