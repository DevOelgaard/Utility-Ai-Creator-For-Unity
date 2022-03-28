using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal interface IPersister
{
    void SaveObject<T>(T o, string path);
    T LoadObject<T>(string path);
    //string GetExtension();
}