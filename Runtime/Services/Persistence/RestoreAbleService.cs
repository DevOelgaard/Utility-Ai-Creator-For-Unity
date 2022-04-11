using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal static class RestoreAbleService
{
    internal static List<string> NamesToList<T>(List<T> aiObjects) where T : AiObjectModel
    {
        var result = new List<string>();
        foreach(var a in aiObjects.Where(a => a != null))
        {
            result.Add(a.Name);
        }

        return result;
    }

    internal static List<string> NamesToList(List<Parameter> aiObjects)
    {
        var result = new List<string>();
        foreach (var a in aiObjects.Where(a => a != null))
        {
            result.Add(a.Name);
        }
        return result;
    }

    internal static List<T> SortByName<T>(List<string> names, List<T> aiObjects) where T : AiObjectModel 
    {
        var result = new List<T>();
        foreach(var name in names)
        {
            var aiObject = aiObjects.FirstOrDefault(a => a.Name == name);
            if(aiObject != null)
            {
                result.Add(aiObject);
                aiObjects.Remove(aiObject);
            }
        }

        foreach(var ao in aiObjects.Where(ao => !result.Contains(ao)))
        {
             result.Add(ao);
        }
        return result;
    }

    internal static List<Parameter> SortByName(List<string> names, List<Parameter> aiObjects)
    {
        var result = new List<Parameter>();
        foreach (var name in names)
        {
            var aiObject = aiObjects.FirstOrDefault(a => a.Name == name);
            if (aiObject != null)
            {
                result.Add(aiObject);
                aiObjects.Remove(aiObject);
            }
        }

        foreach (var ao in aiObjects.Where(ao => !result.Contains(ao)))
        {
            result.Add(ao);
        }
        return result;
    }

    internal static List<T> GetAiObjects<T>(string path, bool restoreDebug) where T : AiObjectModel


    {
        var result = new List<T>();
        var states = PersistenceAPI.Instance.LoadObjectsPathWithFilters<RestoreState>(path, typeof(T));
        foreach (var s in states)
        {
            if (s.LoadedObject == null)
            {
                Debug.Log("Creating error file typeof(T): " + typeof(T) + " s.ModelType: " + s.ModelType + " TType: " + s.type);
                var error = (T)ErrorObjectService.GetErrorObject(s.ModelType);
                error.Name = s.ErrorMessage;
                error.Description = "Exception: " + s.Exception.ToString();
                result.Add(error);
            }
            else
            {
                var model = RestoreAble.Restore<T>(s.LoadedObject, restoreDebug);
                result.Add(model);
            }
        }

        return result;
    }

    internal static List<Parameter> GetParameters(string path, bool restoreDebug)
    {
        var result = new List<Parameter>();
        var parameterStates = PersistenceAPI.Instance.LoadObjectsPathWithFilters<RestoreState>(path, typeof(Parameter));
        foreach (var p in parameterStates)
        {
            if (p.LoadedObject == null)
            {
                var parameter = new Parameter(p.ErrorMessage, p.Exception.ToString());
                result.Add(parameter);
            }
            else
            {
                var parameter = Parameter.Restore<Parameter>(p.LoadedObject, restoreDebug);
                result.Add(parameter);
            }
        }
        return result;
    }

    internal static List<UtilityContainerSelector> GetUCS(string path, bool restoreDebug)
    {
        var result = new List<UtilityContainerSelector>();
        var filter = FileExtensionService.GetFileExtensionFromType(typeof(UtilityContainerSelector));
        var states = PersistenceAPI.Instance.LoadObjectsPath<RestoreState>(path, filter);
        foreach (var bs in states)
        {
            if (bs.LoadedObject == null)
            {
                var error = (UtilityContainerSelector)InstantiaterService.Instance.CreateInstance(bs.ModelType);
                var errorParam = new Parameter(bs.ErrorMessage, bs.Exception.ToString());
                error.Parameters.Add(errorParam);
                result.Add(error);
            }
            else
            {
                var ucs = UtilityContainerSelector.Restore<UtilityContainerSelector>(bs.LoadedObject, restoreDebug);
                result.Add(ucs);
            }
        }
        return result;
    }

}
