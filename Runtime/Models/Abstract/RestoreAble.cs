using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public abstract class RestoreAble
{
    private string fileName;
    public string FileName
    {
        get
        {
            if(fileName == null)
            {
                fileName = GetFileName();
            }
            return fileName;
        }
        protected set
        {
            fileName = value;   
        }
    }
    public Type DerivedType;
    protected RestoreAble()
    {
        DerivedType = GetType();
    }

    public virtual string GetTypeDescription()
    {
        return DerivedType.ToString();
    }

    protected RestoreAble(RestoreAble original)
    {
        DerivedType = original.DerivedType;
        FileName = original.FileName;
    }
    protected abstract string GetFileName();

    public string CurrentDirectory;
    protected abstract void RestoreInternal(RestoreState state, bool restoreDebug = false);
    public static T Restore<T>(RestoreState state, bool restoreDebug = false) where T:RestoreAble
    {
        var type = Type.GetType(state.TypeString);
        T element = default(T);
        if (type == null)
        {
            element = AssetDatabaseService.GetInstanceOfType<T>(state.TypeString);
        } else
        {
            element = (T)InstantiaterService.Instance.CreateInstance(type,true);
        }
        element.CurrentDirectory = state.FolderLocation + "/" + state.FileName + "/";
        element.RestoreInternal(state, restoreDebug);
        return element;
    }

    public static RestoreAble Restore(RestoreState state, Type type, bool restoreDebug = false)
    {
        var element = (RestoreAble)InstantiaterService.Instance.CreateInstance(type, true);
        element.CurrentDirectory = state.FolderLocation + "/" + state.FileName + "/";

        element.RestoreInternal(state, restoreDebug);
        return element;
    }

    internal virtual void SaveToFile(string path, IPersister persister, string fileName = null)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        var state = GetState();
        state.FolderLocation = path;
        if(fileName == null)
        {
            FileName = GetFileName();
        } else
        {
            FileName = fileName;
        }
        path = path + "/" + FileName;

        InternalSaveToFile(path, persister, state);
    }


    protected abstract void InternalSaveToFile(string path, IPersister persister, RestoreState state);

    internal abstract RestoreState GetState();
}

public abstract class RestoreState
{
    public string FileName;
    public string TypeString;
    public string FolderLocation;
    public Type DerivedType;
    public int Index;

    public RestoreState()
    {
    }

    public RestoreState(RestoreAble o)
    {
        FileName = o.FileName;
        DerivedType = o.DerivedType;
        TypeString = o.DerivedType.ToString();
    }
}
