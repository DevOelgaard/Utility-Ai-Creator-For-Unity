using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

internal class JSONDestructivePersister: JSONPersister
{
    protected override void CreateFile(string path)
    {
        var directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(path)).FullName;
        var fileName = Path.GetFileName(path);
        
        if (Directory.Exists(directory))
        {
            Directory.Delete(directory, true);
        }
        Directory.CreateDirectory(directory);

        if (!File.Exists(path))
        {
            var file = File.Create(path);
            file.Close();
        }
    }
}
