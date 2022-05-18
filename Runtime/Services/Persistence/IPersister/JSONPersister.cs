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

    public async Task<ObjectMetaData<T>> LoadObjectAsync<T>(string path)
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
                        IsSuccessFullyLoaded = false
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
                DebugService.LogWarning("Loading failed: " + ex, this);
                var result = new ObjectMetaData<T>(default(T), path)
                {
                    ErrorMessage = "Loading failed at: " + path,
                    Exception = ex,
                    IsSuccessFullyLoaded = false
                };
                return result;
            }
        });
        return await t;
    }
    
    public virtual async Task<List<ObjectMetaData<T>>> LoadObjectsAsync<T>(string folderPath, string searchPattern = null)
    {
        // TODO test if this loading method is faster
            if (!Directory.Exists(folderPath)) return new List<ObjectMetaData<T>>();
            string[] fileNames;
            if (string.IsNullOrEmpty(searchPattern))
            {
                fileNames = Directory.GetFiles(folderPath);
            }
            else
            {
                fileNames = Directory
                    .GetFiles(folderPath, searchPattern);
            }
            
            var tasks = fileNames
                .Where(f => !f.Contains("meta"))
                .Select(file => Task.Run(async () =>
                {
                    var o = await LoadObjectAsync<T>(file);
                    return o;
                }))
                .ToList();

            var taskResult = Task.WhenAll(tasks);
            var result = await taskResult;

            return result.ToList();
    }

    public ObjectMetaData<T> LoadObject<T>(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                var res = new ObjectMetaData<T>(default(T), path)
                {
                    ErrorMessage = "File Doesn't exist",
                    IsSuccessFullyLoaded = false
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
            DebugService.LogWarning("Loading failed: " + ex, this);
            var result = new ObjectMetaData<T>(default(T), path)
            {
                ErrorMessage = "Loading failed at: " + path,
                Exception = ex,
                IsSuccessFullyLoaded = false
            };
            return result;
        }
    }
    public void SaveObject<T>(T o, string path)
    {
        try
        {
            var toJson = JsonConvert.SerializeObject(o, Formatting.Indented, new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            CreateFile(path);
            File.WriteAllText(path, toJson);
            File.SetLastWriteTime(path, DateTime.Now);
        }
        catch(Exception ex)
        {
            throw new Exception("File Not Saved: ", ex);
        }
    }

    public virtual async Task SaveObjectAsync<T>(T o, string path)
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
                File.SetLastWriteTime(path, DateTime.Now);
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