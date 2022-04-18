using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

internal class JsonPersister : IPersister
{
    private string json;

    public async Task<ObjectMetaData<T>> LoadObject<T>(string path)
    {
        var t = Task.Factory.StartNew(() =>
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
            }
            catch (Exception ex)
            {
                Debug.LogWarning("Loading failed: " + ex);
                var result = new ObjectMetaData<T>(default(T), path)
                {
                    ErrorMessage = "Loading failed at: " + path,
                    Exception = ex,
                    Success = false
                };
                return result;
            }
        });
        return await t;
    }
    
    public virtual async Task<List<ObjectMetaData<T>>> LoadObjects<T>(string folderPath, string filter)
    {
            if (!Directory.Exists(folderPath)) return new List<ObjectMetaData<T>>();
            var fileNames = Directory
                .GetFiles(folderPath, filter);
            var result = new List<ObjectMetaData<T>>();
            foreach (var file in fileNames.Where(f => !f.Contains("meta")))
            {
                result.Add(await LoadObject<T>(file));
            }

            return result;
    }

    public virtual async Task SaveObject<T>(T o, string path)
    {
        var t = Task.Factory.StartNew(() =>
        {
            try
            {
                var toJson = JsonConvert.SerializeObject(o, Formatting.Indented, new JsonSerializerSettings
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
        });
        await t;
    }

    protected virtual void CreateFile(string path)
    {
        var directory = new DirectoryInfo(Path.GetDirectoryName(path) ?? string.Empty).FullName;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (File.Exists(path)) return;
        var file = File.Create(path);
        file.Close();
    }
}