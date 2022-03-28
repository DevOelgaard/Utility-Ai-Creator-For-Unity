using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

internal class JSONPersister : IPersister
{
    //public string GetExtension()
    //{
    //    return ".json";
    //}

    public T LoadObject<T>(string path)
    {
        try
        {
            if (!path.Contains(Consts.FileExtension_JSON))
            {
                path += Consts.FileExtension_JSON;
            }

            if (!File.Exists(path)) return default(T);

            
            var json = File.ReadAllText(path);
            var deserialized = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            return deserialized;
        } catch(Exception ex)
        {
            Debug.LogWarning("Loading failed: " + ex.ToString());
            throw new Exception("Loading failed: ", ex);

            //return default(T);
        }
    }

    public void SaveObject<T>(T o, string path)
    {
        try
        {
            if (!path.Contains(Consts.FileExtension_JSON))
            {
                path += Consts.FileExtension_JSON;
            }
            var toJson = JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
            CreateFile(path);
            File.WriteAllText(path, toJson);
        }
        catch(Exception ex)
        {
            throw new Exception("File Not Saved: ", ex);
        }
    }

    private void CreateFile(string path)
    {
        if (!path.Contains(Consts.FileExtension_JSON))
        {
            path += Consts.FileExtension_JSON;
        }
        var directory = Path.GetDirectoryName(path);
        var fileName = Path.GetFileName(path);
        if (!File.Exists(directory))
        {
            Directory.CreateDirectory(directory);
            var file = File.Create(directory + fileName);
            file.Close();
        }
    }
}