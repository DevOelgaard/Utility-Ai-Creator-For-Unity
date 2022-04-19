using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPersister
{
    Task SaveObject<T>(T o, string path);
    Task<ObjectMetaData<T>> LoadObjectAsync<T>(string path);
    Task<List<ObjectMetaData<T>>> LoadObjects<T>(string folderPath, string filter);
    ObjectMetaData<T> LoadObject<T>(string path);
}