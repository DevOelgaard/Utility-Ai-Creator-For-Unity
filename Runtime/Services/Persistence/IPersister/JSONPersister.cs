using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

internal class JSONPersister : IPersister
{
    private string json;
    public ObjectMetaData<T> LoadObject<T>(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                var res = new ObjectMetaData<T>(default(T), path);
                res.ErrorMessage = "File Doesn't exist";
                res.Success = false;
                return res;
            }
            
            json = File.ReadAllText(path);
            var deserialized = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });

            return new ObjectMetaData<T>(deserialized, path);
        } catch(Exception ex)
        {
            Debug.LogWarning("Loading failed: " + ex.ToString());
            var result = new ObjectMetaData<T>(default(T), path);
            result.ErrorMessage = "Loading failed at: " + path;
            result.Exception = ex;
            result.Success = false;
            return result;
        }
    }

    public virtual List<ObjectMetaData<T>> LoadObjects<T>(string folderPath, string filter)
    {
        try
        {
            var result = new List<ObjectMetaData<T>>();
            var fileNames = Directory.GetFiles(folderPath, filter);
            foreach (var file in fileNames.Where(f => !f.Contains("meta")))
            {
                var t = LoadObject<T>(file);
                result.Add(t);
            }
            return result;
        }
        catch
        {
            return new List<ObjectMetaData<T>>();
        }
    }


    public virtual void SaveObject<T>(T o, string path)
    {
        try
        {
            var toJson = JsonConvert.SerializeObject(o, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            CreateFile(path);
            File.WriteAllText(path, toJson);
        }
        catch(Exception ex)
        {
            throw new Exception("File Not Saved: ", ex);
        }
    }

    protected virtual void CreateFile(string path)
    {
        
        var directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(path)).FullName;
        var fileName = Path.GetFileName(path);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        if (!File.Exists(path))
        {
            var file = File.Create(path);
            file.Close();
        }
    }
}