using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

internal class JsonPersister : IPersister
{
    private string json;
    public ObjectMetaData<T> LoadObject<T>(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                var res = new ObjectMetaData<T>(default(T), path)
                {
                    ErrorMessage = "File Doesn't exist",
                    Success = false
                };
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
            var result = new ObjectMetaData<T>(default(T), path)
            {
                ErrorMessage = "Loading failed at: " + path,
                Exception = ex,
                Success = false
            };
            return result;
        }
    }

    public virtual List<ObjectMetaData<T>> LoadObjects<T>(string folderPath, string filter)
    {
        try
        {
            var fileNames = Directory
                .GetFiles(folderPath, filter);
            return fileNames
                .Where(f => !f.Contains("meta"))
                .Select(LoadObject<T>)
                .ToList();
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
        
        var directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(path) ?? string.Empty).FullName;
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