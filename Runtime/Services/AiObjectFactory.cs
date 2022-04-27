using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

internal class AiObjectFactory
{
    private Dictionary<Type, Delegate> instantiatersByType = new Dictionary<Type, Delegate>();
    private Dictionary<Type, int> instantiationTimes = new Dictionary<Type, int>();
    private static AiObjectFactory _instance;
    public static AiObjectFactory Instance
    {
        get { return _instance ??= new AiObjectFactory(); }
    }

    internal static object CreateInstance(Type t, bool nonPublic = false)
    {
        DebugService.Log("Creating instance of type: " + t, nameof(AiObjectFactory));
        var newObject = t == typeof(Parameter) ? 
            new Parameter() : 
            Activator.CreateInstance(t);

        if (newObject.GetType().GetInterface(nameof(IInitializeAble)) != null)
        {
            DebugService.Log("Initializing: " + t, nameof(AiObjectFactory));
            var cast = newObject as IInitializeAble;
            cast?.Initialize();
            return cast;
        }
        
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

        DebugService.Log(msg, this);

    }

    ~AiObjectFactory()
    {
    }
}

