using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
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

    internal void SaveObjectPanel(RestoreAble o)
    {
        var extension = FileExtensionService.GetExtension(o);
        var path = EditorUtility.SaveFilePanel("Save object", "", "name", extension);
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        SaveObjectPath(o, Path.GetDirectoryName(path) + @"\", Path.GetFileName(path));
    }

    internal void SaveObjectsPanel(List<RestoreAble> restoreables)
    {
        var path = EditorUtility.SaveFolderPanel("Save object", "folder", "default name");
        foreach (var r in restoreables)
        {
            SaveObjectPath(r, path, r.FileName);
        }
    }

    internal void SaveObjectPath(RestoreAble o, string path, string fileName)
    {
        o.SaveToFile(path, persister, -2, fileName);
    }

    internal void SaveDestructiveObjectPath(RestoreAble o, string path, string fileName)
    {
        o.SaveToFile(path, destructivePersister,-2, fileName);
        CleanUp(path);
    }

    internal ObjectMetaData<T> LoadObjectPanel<T>()
    {
        var extension = FileExtensionService.GetExtension(typeof(T));
        var path = EditorUtility.OpenFilePanel("Load object", "", extension);
        var o = persister.LoadObject<T>(path);
        return o;
    }

    internal ObjectMetaData<T> LoadObjectPanel<T>(string[] filters)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Load object", "", filters);
        var o = persister.LoadObject<T>(path);
        return o;
    }

    internal ObjectMetaData<T> LoadObjectPath<T>(string path)
    {
        var result = persister.LoadObject<T>(path);
        // CleanUp(path);
        return result;
    }

    internal List<ObjectMetaData<T>> LoadObjectsPanel<T>(string startPath, string filter = "") where T : RestoreState
    {
        var path = EditorUtility.OpenFolderPanel("Load object", startPath, "default name");
        return persister.LoadObjects<T>(path, "*"+filter);
    }

    internal List<ObjectMetaData<T>> LoadObjectsPath<T>(string folderPath, string filter = "") where T: RestoreState
    {
        var results = persister.LoadObjects<T>(folderPath, "*"+filter);
        foreach (var result in results)
        {
            if (result.LoadedObject == null) continue;
            result.LoadedObject.FolderLocation = folderPath;
        }

        return results;
    }

    internal List<ObjectMetaData<T>> LoadObjectsPathWithFilters<T>(string folderPath, Type t) where T : RestoreState
    {
        var filter = FileExtensionService.GetFileExtensionFromType(t);
        return LoadObjectsPath<T>(folderPath, filter);
    }

    internal List<ObjectMetaData<T>> LoadObjectsPathWithFiltersAndSubDirectories<T>(string folderPath, Type t) where T : RestoreState
    {
        var filter = FileExtensionService.GetFileExtensionFromType(t);

        var result = LoadObjectsPath<T>(folderPath, filter);
        try
        {
            var subDirectories = Directory.GetDirectories(folderPath);
            result
                .AddRange(subDirectories
                    .SelectMany(subDirectory => LoadObjectsPath<T>(subDirectory, filter)));
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

    internal ObjectMetaData<T> LoadFilePanel<T>(string[] filters)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Load object", "", filters);
        return LoadFilePath<T>(path);
    }

    private ObjectMetaData<T> LoadFilePath<T>(string path)
    {
        return persister.LoadObject<T>(path);
    }

    //https://stackoverflow.com/questions/2811509/c-sharp-remove-all-empty-subdirectories
    private void CleanUp(string path)
    {
        var thread = Thread.CurrentThread;
        Debug.Log(thread.Name + " Starting Clea Up path: " + path);

        if (path.Contains("."))
        {
            path = new DirectoryInfo(System.IO.Path.GetDirectoryName(path) ?? string.Empty).FullName;
        }
        var directories = Directory.GetDirectories(path);
        foreach (var d in directories.Where((d => !d.Contains("."))))
        {
            CleanUp(d);

            var metaFiles = Directory.GetFiles(d).Where(f => f.Contains(".meta"));
            var fileNamesWithoutMeta = Directory.GetFiles(d)
                .Where(f => !f.Contains(".meta"))
                .Select(Path.GetFileName)
                .ToList();
            var childDirectoryNames = Directory.GetDirectories(d)
                .Select(Path.GetFileNameWithoutExtension)
                .ToList();
            var deleted = new List<string>();
            foreach (var metaFile in metaFiles)
            {
                var metaFileName = Path.GetFileNameWithoutExtension(metaFile);
                var canDelete = fileNamesWithoutMeta.All(file => file != metaFileName) &&
                                childDirectoryNames.All(directory => directory != metaFileName);

                if (!canDelete) continue;
                deleted.Add(metaFile);
                File.Delete(metaFile);
            }

            // Delete empty folders
            var childDirectories = Directory.GetDirectories(d);
            if (childDirectories.Length > 0) continue;

            var filesWithoutMeta = Directory.GetFiles(d).Where(f => !f.Contains(".meta")).ToList();
            if (filesWithoutMeta.Count > 0) continue;
            foreach (var file in Directory.GetFiles(d))
            {
                deleted.Add(file);
                File.Delete(file);
            }

            Directory.Delete(d,false);
            Debug.Log(thread.Name + " Clean Up of: " + d + " completed Deleted files: " + deleted.Count);
            foreach (var del in deleted)
            {
                Debug.Log(thread.Name + " Deleted: " + del);
            }
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