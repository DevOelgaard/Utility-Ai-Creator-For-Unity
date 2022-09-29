using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NSubstitute.Exceptions;

namespace Mocks
{
    public class Mock_IPersister: IPersister
    {
        public int SaveObjectAsyncCount = 0;
        public int SaveObjectCount = 0;
        public int LoadObjectAsyncCount = 0;
        public int LoadObjectsCount = 0;
        public int LoadObjectCount = 0;
        
        public Task SaveObjectAsync<T>(T o, string path)
        {
            throw new System.NotImplementedException();
        }

        public void SaveObject<T>(T o, string path)
        {
            SaveObjectCount++;
        }

        public Task<ObjectMetaData<T>> LoadObjectAsync<T>(string path)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<ObjectMetaData<T>>> LoadObjectsAsync<T>(string folderPath, string searchPattern)
        {
            throw new System.NotImplementedException();
        }

        public ObjectMetaData<T> LoadObject<T>(string path)
        {
            LoadObjectCount++;
            return null;
        }

        public ObjectMetaData<object> LoadObject(string path, Type t)
        {
            throw new NotImplementedException();
        }
    }
}