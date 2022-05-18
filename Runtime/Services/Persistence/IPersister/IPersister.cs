using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPersister
{
    Task SaveObjectAsync<T>(T o, string path);
    void SaveObject<T>(T o, string path);
    Task<ObjectMetaData<T>> LoadObjectAsync<T>(string path);
    Task<List<ObjectMetaData<T>>> LoadObjectsAsync<T>(string folderPath, string searchPattern);
    ObjectMetaData<T> LoadObject<T>(string path);
}