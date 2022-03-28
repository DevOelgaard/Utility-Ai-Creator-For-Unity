using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class RestoreAble
{
    protected abstract void RestoreInternal(RestoreState state, bool restoreDebug = false);
    public static T Restore<T>(RestoreState state, bool restoreDebug = false) where T:RestoreAble
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        var type = Type.GetType(state.FileName);
        if (type == null)
        {
            var e = AssetDatabaseService.GetInstanceOfType<T>(state.FileName);
            sw.Restart();

            e.RestoreInternal(state, restoreDebug);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "Restore Type = null");

            return e;
        } else
        {
            var element = (T)InstantiaterService.Instance.CreateInstance(type,true);
            sw.Restart();
            //var element = (T)Activator.CreateInstance(type, true);
            element.RestoreInternal(state, restoreDebug);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "Restore Type != null");
            return element;
        }
    }

    internal virtual void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }

    internal abstract RestoreState GetState();
}

public class RestoreState
{
    public string FileName;

    public RestoreState()
    {
    }

    public RestoreState(RestoreAble o)
    {
        FileName = TypeDescriptor.GetClassName(o);
    }
}
