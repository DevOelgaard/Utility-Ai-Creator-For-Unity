using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using UnityEngine;
using static System.LambdaActivator;

internal class InstantiaterService
{
    private Dictionary<Type, Delegate> instantiatersByType = new Dictionary<Type, Delegate>();
    private Dictionary<Type, int> instantiationTimes = new Dictionary<Type, int>();
    private static InstantiaterService _instance;
    public static InstantiaterService Instance
    {
        get { return _instance ??= new InstantiaterService(); }
    }

    internal static object CreateInstance(Type t, bool nonPublic = false)
    {
        var newObject = t == typeof(Parameter) ? 
            new Parameter() : 
            Activator.CreateInstance(t);
        
        return newObject;
    }

    private Delegate GetDelegate(Type t)
    {
        if (!instantiatersByType.ContainsKey(t))
        {
            var ctor = t.GetConstructors().First();
            var activator = LambdaActivator.GetActivator(ctor);
            instantiatersByType.Add(t, activator);
            instantiationTimes.Add(t, 0);
        } else
        {
            instantiationTimes[t]++;
        }

        return instantiatersByType[t];
    }

    internal void Reset()
    {
        instantiationTimes = new Dictionary<Type, int>();
        instantiatersByType = new Dictionary<Type, Delegate>();
    }

    internal void DebugStuff()
    {
        var list = new List<KeyValuePair<Type, int>>();
        foreach(var v in instantiationTimes)
        {
            list.Add(new KeyValuePair<Type, int>(v.Key, v.Value));
        }
        var ordered = list.OrderByDescending(kv => kv.Value);
        var msg = "";
        foreach(var kv in ordered)
        {
            msg += kv.Key + ": " + kv.Value + " | ";
        }

        Debug.Log("InstantiaterService: " + msg);

    }

    ~InstantiaterService()
    {
    }
}

