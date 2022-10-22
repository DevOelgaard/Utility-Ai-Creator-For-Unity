using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

internal class AiObjectFactory
{
    private static AiObjectFactory _instance;
    public static AiObjectFactory Instance
    {
        get { return _instance ??= new AiObjectFactory(); }
    }

    internal static object CreateInstance(Type t, bool nonPublic = false)
    {
        DebugService.Log("Creating instance of type: " + t, nameof(AiObjectFactory));
        // var newObject = t == typeof(Parameter) ? 
        //     new Parameter() : 
        //     Activator.CreateInstance(t);
        var newObject = Activator.CreateInstance(t);

        if (newObject.GetType().GetInterface(nameof(IInitializeAble)) != null)
        {
            DebugService.Log("Initializing: " + t, nameof(AiObjectFactory));
            var cast = newObject as IInitializeAble;
            cast?.Initialize();
            return cast;
        }
        
        return newObject;
    }
}

