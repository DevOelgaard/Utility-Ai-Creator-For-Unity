using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IPersister
{
    void SaveObject<T>(T o, string path);
    ObjectMetaData<T> LoadObject<T>(string path);
    List<ObjectMetaData<T>> LoadObjects<T>(string folderPath, string filter);
}