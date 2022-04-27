using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public static class AssetDatabaseService
{
    private static readonly Dictionary<string,string> cachedPaths = new Dictionary<string,string>();
    private static readonly Dictionary<string,VisualTreeAsset> cachedVisualTrees = new Dictionary<string, VisualTreeAsset>();
    private static BinaryFormatter _binaryFormatter = new BinaryFormatter();
    public static string GetAssetPath(string filter, string type)
    {
        if (cachedPaths.ContainsKey(filter + type))
        {
            return cachedPaths[filter + type];
        } else
        {
            var GUIDS = AssetDatabase.FindAssets(filter);

            var paths = new List<string>();
            foreach (var guid in GUIDS)
            {
                paths.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            foreach (var p in paths)
            {
                if (p.Contains(type))
                {
                    cachedPaths.Add(filter+ type, p);
                    return p;
                }
            }

            return null;
        }
    }

    public static VisualTreeAsset GetVisualTreeAsset(string name)
    {
        if (name.Contains("`"))
        {
            name = name.Substring(0, name.IndexOf("`"));
        }
        if (!cachedVisualTrees.ContainsKey(name))
        {
            var path = AssetDatabaseService.GetAssetPath(name, "uxml");
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            cachedVisualTrees.Add(name, visualTreeAsset);
        } 
        return cachedVisualTrees[name];
    }

    public static TemplateContainer GetTemplateContainer(Type type) {
        return GetTemplateContainer(TypeDescriptor.GetClassName(type));
    }
    public static TemplateContainer GetTemplateContainer(string name)
    {
        var template = GetVisualTreeAsset(name);
        return template.CloneTree();
    }

    public static T GetFirstInstanceOfType<T>()
    {
        var assemblies = GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (typeof(T).IsAssignableFrom(type) &&
                    !type.IsAbstract)
                {
                    // Guarding against Test files
                    if (!type.ToString().Contains("Mock") &&
                        !type.ToString().Contains("Stub"))
                    {
                        return (T)AiObjectFactory.CreateInstance(type);
                        //var instance = (T)Activator.CreateInstance(type);
                    }
                }
            }
        }
        return default(T);
    }

    public static List<Type> GetAssignableTypes<T>()
    {
        var result = new List<Type>();
        var assemblies = GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (typeof(T).IsAssignableFrom(type) &&
                    !type.IsAbstract)
                {
                    // Guarding against Test files
                    if (!type.ToString().Contains("Mock") &&
                        !type.ToString().Contains("Stub"))
                    {
                        result.Add(type);
                    }
                }
            }
        }
        return result;
    }

    public static List<T> GetInstancesOfType<T>()
    {
        var result = new List<T>();
        var assemblies = GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (typeof(T).IsAssignableFrom(type) &&
                    !type.IsAbstract)
                {
                    // Guarding against Test files
                    if (!type.ToString().Contains("Mock") &&
                        !type.ToString().Contains("Stub"))
                    {
                        var instance = (T)AiObjectFactory.CreateInstance(type);
                        result.Add(instance);
                    }
                }
            }
        }
        return result;
    }

    public static List<Type> GetActivateAbleTypes(Type t)
    {
        var result = new List<Type>();
        var assemblies = GetAssemblies();

        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                if (t.IsAssignableFrom(type) &&
                    !type.IsAbstract)
                {
                    result.Add(type);
                }
            }
        }

        return result;
    }

    public static T GetInstanceOfType<T>(string typeName)
    {
        var assemblies = GetAssemblies();
        foreach (var assembly in assemblies)
        {
            var types = assembly.GetTypes();

            var type = types.FirstOrDefault(t => t.ToString() == typeName);
            if (type != null)
            {
                var instance = (T)AiObjectFactory.CreateInstance(type);
                // if (type.GetInterface(nameof(IInitializeAble)) != null)
                // {
                //     var cast = instance as IInitializeAble;
                //     cast?.Initialize();
                //     return (T)cast;
                // }

                return instance;
            }
        }
        return default(T);
    }

    /// <summary>
    /// https://stackoverflow.com/questions/11336935/c-sharp-automatic-deep-copy-of-struct
    /// Works slow but makes deep copy of object
    /// </summary>
    /// <param name="obj"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    internal static T DeepCopy<T>(T obj)
    {
        using var ms = new MemoryStream();
        _binaryFormatter.Serialize(ms,obj);
        ms.Position = 0;
        var t = (T) _binaryFormatter.Deserialize(ms);
        return t;
    }

    //public static object GetInstanceOfType(Type type)
    //{
    //    var assemblies = GetAssemblies();
    //    foreach (var assemblie in assemblies)
    //    {
    //        var types = assemblie.GetTypes();

    //        if (type != null)
    //        {
    //            var instance = InstantiaterService.Instance.CreateInstance(type);

    //            return instance;
    //        }
    //    }
    //    return null;
    //}

    private static System.Reflection.Assembly[] GetAssemblies()
    {
        //var assemblies = System.AppDomain.CurrentDomain.GetAssemblies().ToList();
        //var newAssemblies = assemblies.Where(a => !a.GetName().ToString().Contains("UnityEngine")).ToList();
        //var newAssemblies2 = newAssemblies.Where(a => !a.GetName().ToString().Contains("UnityEditor")).ToArray();

        //return newAssemblies2;
       return System.AppDomain.CurrentDomain.GetAssemblies();

    }
}
