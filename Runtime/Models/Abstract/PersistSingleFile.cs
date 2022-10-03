using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

public abstract class PersistSingleFile
{
    public readonly Type DerivedType;

    protected PersistSingleFile()
    {
        DerivedType = GetType();
    }
    
    protected abstract Task RestoreFromFile(SingleFileState state);

    public static async Task<PersistSingleFile> Restore(SingleFileState state, Type t)
    {
        var element = (PersistSingleFile)AiObjectFactory.CreateInstance(t, true);

        try
        {
            await element.RestoreFromFile(state);
            element.OnRestoreComplete();
            return element;
        }
        catch (Exception e)
        {
            DebugService.LogError("Failed to load " + state?.AssemblyQualifiedName, nameof(PersistSingleFile), e);
            return element;
        }
        
    }
    public static async Task<T> Restore<T>(SingleFileState state) where T:PersistSingleFile
    {
        try
        {
            var type = Type.GetType(state.AssemblyQualifiedName);
            T element = default(T);
            if (type == null)
            {
                element = AssetService.GetInstanceOfType<T>(state.AssemblyQualifiedName);
            } else
            {
                element = (T)AiObjectFactory.CreateInstance(type,true);
            }
            await element.RestoreFromFile(state);
            element.OnRestoreComplete();
            return element;
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    public virtual async Task SaveToFile(string path)
    {
        DebugService.Log("Start SaveToSingleFile path: " + path, this, Thread.CurrentThread);

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var state = GetSingleFileState();
        var persister = new JsonPersister();
        await persister.SaveObjectAsync(state,path);

        DebugService.Log("Done SaveToSingleFile path: " + path, this, Thread.CurrentThread);
    }

    public abstract SingleFileState GetSingleFileState();
    
    protected virtual void OnRestoreComplete(){}

    public virtual string GetTypeDescription()
    {
        return "No TypeDescription Set";
    }
}

public abstract class SingleFileState
{
    public string AssemblyQualifiedName;
    public Type OriginalType => Type.GetType(AssemblyQualifiedName);

    protected SingleFileState()
    {
    }
    protected SingleFileState(PersistSingleFile o)
    {
        AssemblyQualifiedName = o.GetType().AssemblyQualifiedName;
    }
}