using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

public abstract class RestoreAble
{
    public string FileName => GetFileName();
    public readonly Type DerivedType;
    protected RestoreAble()
    {
        DerivedType = GetType();
    }

    public virtual string GetTypeDescription()
    {
        return DerivedType.ToString();
    }

    protected abstract string GetFileName();

    public string CurrentDirectory;
    protected abstract Task RestoreInternalAsync(RestoreState state, bool restoreDebug = false);
    public static async Task<T> Restore<T>(RestoreState state, bool restoreDebug = false) where T:RestoreAble
    {
        try
        {
            var type = Type.GetType(state.AssemblyQualifiedName);
            T element = default(T);
            if (type == null)
            {
                element = AssetDatabaseService.GetInstanceOfType<T>(state.AssemblyQualifiedName);
            } else
            {
                element = (T)AiObjectFactory.CreateInstance(type,true);
            }
            element.CurrentDirectory = state.FolderLocation + "/" + state.FileName + "/";
            DebugService.Log("Setting CurrentDirectory: " + element.CurrentDirectory + " of type: " + typeof(T), nameof(RestoreAble));
            await element.RestoreInternalAsync(state, restoreDebug);
            element.OnRestoreComplete();
            return element;
        }
        catch
        {
            DebugService.LogError("Failed to restore: " + state.FileName + " path: " + state.FolderLocation, nameof(RestoreAble));
            throw;
        }
    }
    
    protected virtual void OnRestoreComplete(){}

    public static async Task<RestoreAble> Restore(RestoreState state, Type type, bool restoreDebug = false)
    {
        var element = (RestoreAble)AiObjectFactory.CreateInstance(type, true);
        element.CurrentDirectory = state.FolderLocation + "/" + state.FileName + "/";

        await element.RestoreInternalAsync(state, restoreDebug);
        return element;
    }

    internal virtual async Task SaveToFile(string path, IPersister persister, int index = -1, string className = null)
    {
        var task = Task.Factory.StartNew(async () =>
        {
            DebugService.Log("Start SaveToFile path: " + path, this, Thread.CurrentThread);

            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            var state = GetState();
            path = path + "/" + FileName;
            state.Index = index;
            state.FolderLocation = Path.GetDirectoryName(path);
            DebugService.Log("Setting folder location of " + state.FileName + " Folder location: " + state.FolderLocation + " Path: " + path, this);


            await InternalSaveToFile(path, persister, state);
            DebugService.Log("Done SaveToFile path: " + path, this, Thread.CurrentThread);
        });
        await task;
    }


    protected abstract Task InternalSaveToFile(string path, IPersister persister, RestoreState state);

    internal abstract RestoreState GetState();
    
    public override string ToString()
    {
        return "FileName: " + FileName + " DerivedType: " + DerivedType + " CurrentDirectory: " + CurrentDirectory;
    }
}

public abstract class RestoreState
{
    public string FileName;
    // public string DerivedTypeString;
    public string FolderLocation;
    // public Type DerivedType;
    public int Index;
    public string AssemblyName;
    public string AssemblyQualifiedName;

    public RestoreState()
    {
    }

    public RestoreState(RestoreAble o)
    {
        FileName = o.FileName;
        // DerivedType = o.DerivedType;
        // DerivedTypeString = o.DerivedType.ToString();
        AssemblyName = o.DerivedType.Assembly.FullName;
        AssemblyQualifiedName = o.DerivedType.AssemblyQualifiedName;
    }

    public Type OriginalType => Type.GetType(AssemblyQualifiedName);
}
