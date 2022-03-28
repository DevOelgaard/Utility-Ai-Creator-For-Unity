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

    internal PersistenceAPI(IPersister persister)
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
        o.SaveToFile(path, persister);
        //persister.SaveObject(o, path);
    }

    internal void SaveObjectPath(RestoreAble o, string path)
    {
        o.SaveToFile(path, persister);
    }

    internal T LoadObjectPanel<T>()
    {
        var extension = FileExtensionService.GetExtension(typeof(T));
        var path = EditorUtility.OpenFilePanel("Load object", "", extension);
        var o = persister.LoadObject<T>(path);
        return o;
    }

    internal T LoadObjectPanel<T>(string[] filters)
    {
        var path = EditorUtility.OpenFilePanelWithFilters("Load object", "", filters);
        var o = persister.LoadObject<T>(path);
        return o;
    }

    internal T LoadObjectPath<T>(string path)
    {
        return persister.LoadObject<T>(path);
    }
}