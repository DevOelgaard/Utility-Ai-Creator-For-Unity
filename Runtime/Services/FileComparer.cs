// https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-compare-the-contents-of-two-folders-linq

using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;

public class FileComparer: System.Collections.Generic.IEqualityComparer<System.IO.FileInfo> 
{
    public FileComparer()
    {
    }

    public bool Equals(FileInfo f1, FileInfo f2)
    {
        return (f1.Name == f2.Name &&
                f1.Length == f2.Length);
    }

    public int GetHashCode(FileInfo fi)
    {
        var s = $"{fi.Name}{fi.Length}";
        return s.GetHashCode();
    }
}