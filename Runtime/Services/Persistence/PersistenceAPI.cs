using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

internal class PersistenceAPI
{
    private IPersister persister;
    private IPersister destructivePersister;

    internal static PersistenceAPI Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new PersistenceAPI(new JSONPersister());
            }
            return instance;
        }
    }
    private static PersistenceAPI instance;

    private PersistenceAPI(IPersister persister)
    {
        this.persister = persister;
        this.destructivePersister = new JSONDestructivePersister();
    }

    internal void SetPersister(IPersister persister)
    {
        this.persister = persister;
    }

    internal void SaveObjectPanel(RestoreAble o)
    {
        var extension = FileExtensionService.GetExtension(o);
        var path = EditorUtility.SaveFilePanel("Save object", "", "name", extension);
        if (path == null || path.Length == 0)
        {
            return;
        }
        SaveObjectPath(o, Path.GetDirectoryName(path) + @"\", Path.GetFileName(path));
        //o.SaveToFile();
        //persister.SaveObject(o, path);
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
        o.SaveToFile(path, persister, fileName);
    }

    internal void SaveDestructiveObjectPath(RestoreAble o, string path, string fileName)
    {
        o.SaveToFile(path, destructivePersister, fileName);
    }


    internal void SaveDestructivelyPath(RestoreAble o, string path)
    {

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
        return persister.LoadObject<T>(path);
    }

    internal List<ObjectMetaData<T>> LoadObjectsPanel<T>(string startPath, string filter = "") where T : RestoreState
    {
        var path = EditorUtility.OpenFolderPanel("Load object", startPath, "default name");
        return persister.LoadObjects<T>(path, "*"+filter);
    }

    internal List<ObjectMetaData<T>> LoadObjectsPath<T>(string folderPath, string filter = "") where T: RestoreState
    {
        return persister.LoadObjects<T>(folderPath, "*"+filter);
    }

    internal List<ObjectMetaData<T>> LoadObjectsPathWithFilters<T>(string folderPath, Type t) where T : RestoreState
    {
        var filter = FileExtensionService.GetFileExtensionFromType(t);
        return LoadObjectsPath<T>(folderPath, filter);
    }


    internal ObjectMetaData<T> LoadFilePanel<T>(string[] filters)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Load object", "", filters);
        return LoadFilePath<T>(path);
    }

    internal ObjectMetaData<T> LoadFilePath<T>(string path)
    {
        return persister.LoadObject<T>(path);
    }


}

public class ObjectMetaData<T>
{
    public bool Success = true;
    public Type StateType;
    public Type ModelType;
    public T LoadedObject;
    public Type TType;
    public string Path;
    public string ErrorMessage;
    public Exception Exception;

    public ObjectMetaData(T o, string path)
    {
        StateType = FileExtensionService.GetStateFromFileName(path);
        ModelType = FileExtensionService.GetTypeFromFileName(path);
        TType = typeof(T);
        LoadedObject = o;
        Path = path;
    }
}