using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
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
        return aiObjects;
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

    internal static async Task<List<T>> GetAiObjects<T>(string path, bool restoreDebug) where T : AiObjectModel
    {
        var result = new List<T>();
        var states = await PersistenceAPI.Instance.LoadObjectsPathWithFilters<RestoreState>(path, typeof(T));
        states = states.OrderBy(s => s.LoadedObject?.Index).ToList();
        
        foreach (var s in states)
        {
            if (s.LoadedObject == null)
            {
                Debug.Log("Creating error file typeof(T): " + typeof(T) + " s.ModelType: " + s.ModelType + " TType: " + s.type);
                var error = (T)ErrorObjectService.GetErrorObject(s.ModelType);
                error.Name = "Error";
                error.Description = "Exception: " + s.Exception.ToString();
                result.Add(error);
            }
            else
            {
                var model = await RestoreAble.Restore<T>(s.LoadedObject, restoreDebug);
                result.Add(model);
            }
        }

        return result;
    }

    internal static async Task<List<Parameter>> GetParameters(string path, bool restoreDebug)
    {
        var result = new List<Parameter>();
        var parameterStates = await PersistenceAPI.Instance.LoadObjectsPathWithFilters<RestoreState>(path, typeof(Parameter));
        parameterStates = parameterStates.OrderBy(p => p.LoadedObject?.Index).ToList();
        
        foreach (var p in parameterStates)
        {
            if (p.LoadedObject == null)
            {
                var parameter = new Parameter("Error", p.Exception.ToString());
                result.Add(parameter);
            }
            else
            {
                var parameter = await Parameter.Restore<Parameter>(p.LoadedObject, restoreDebug);
                result.Add(parameter);
            }
        }
        return result;
    }
    
    internal static async Task<List<UtilityContainerSelector>> GetUcs(string path, bool restoreDebug)
    {
        var result = new List<UtilityContainerSelector>();
        var filter = FileExtensionService.GetFileExtensionFromType(typeof(UtilityContainerSelector));
        var states = await PersistenceAPI.Instance.LoadObjectsPath<RestoreState>(path, filter);
        states = states.OrderBy(s => s.LoadedObject?.Index).ToList();
        foreach (var bs in states)
        {
            if (bs.LoadedObject == null)
            {
                var error = (UtilityContainerSelector)InstantiaterService.CreateInstance(bs.ModelType);
                var errorParam = new Parameter("Error", bs.Exception.ToString());
                error.Parameters.Add(errorParam);
                result.Add(error);
            }
            else
            {
                var ucs = await RestoreAble.Restore<UtilityContainerSelector>(bs.LoadedObject, restoreDebug);
                result.Add(ucs);
            }
        }
        return result;
    }
    
    internal static async Task LoadObjectsAndSortToCollection<T>(string path, List<string> namesOrdered, 
        ReactiveList<AiObjectModel> collection, bool restoreDebug) where T: AiObjectModel
    {
        var aiObjects = await GetAiObjects<T>(path, restoreDebug);
        var sorted = SortByName(namesOrdered, aiObjects);
        collection.Add(sorted);
    }

    internal static async Task SaveRestoreAblesToFile<T>(IEnumerable<T> collection, string path, IPersister persister) where T: RestoreAble
    {
        var restoreAbles = collection.ToList();
        foreach (var element in restoreAbles)
        {
            await element.SaveToFile(path,persister,restoreAbles.IndexOf(element));
        }
    }
}
