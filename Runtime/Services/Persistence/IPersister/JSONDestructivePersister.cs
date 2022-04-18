using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using UnityEngine;

internal class JsonDestructivePersister: JsonPersister
{
    protected override void CreateFile(string path)
    {
        var directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(path) ?? string.Empty).FullName;
        var fileName = Path.GetFileName(path);
        
        if (Directory.Exists(directory))
        {
            var files = Directory.GetFiles(directory, "*", SearchOption.AllDirectories);
            foreach (var file in files.Where(f => !f.Contains(".meta")))
            {
                File.Delete(file);
            }
            // Directory.Delete(directory, true);
        }
        Directory.CreateDirectory(directory);

        if (!File.Exists(path))
        {
            var file = File.Create(path);
            file.Close();
        }

        var thread = Thread.CurrentThread;
        Debug.Log(thread.Name + " Create File complete");
    }
}
