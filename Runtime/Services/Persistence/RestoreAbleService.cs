using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRxExtension;

internal static class RestoreAbleService
{
    internal static List<string> NamesToList<T>(IEnumerable<T> aiObjects) where T : AiObjectModel
    {
        return aiObjects.Where(a => a != null).Select(a => a.Name).ToList();
    }

    // internal static List<string> NamesToList(List<Parameter> aiObjects)
    // {
    //     return aiObjects.Where(a => a != null).Select(a => a.Name).ToList();
    // }

    internal static List<T> OrderByNames<T>(List<string> desiredOrder, List<T> aiObjects) where T : AiObjectModel
    {
        return aiObjects;
    }

    // internal static async Task<List<T>> GetAiObjectsSortedByIndex<T>(string path, bool restoreDebug) where T : AiObjectModel
    // {
    //     var result = new List<T>();
    //     var states = await PersistenceAPI.Instance.LoadObjectsOfTypeAsync<RestoreState>(path, typeof(T));
    //     states = states.OrderBy(s => s.LoadedObject?.Index).ToList();
    //     
    //     foreach (var s in states)
    //     {
    //         if (s.LoadedObject == null)
    //         {
    //             DebugService.Log("Creating error file typeof(T): " + typeof(T) + " s.ModelType: " + s.ModelType + " TType: " + s.type, nameof(RestoreAbleService));
    //             var error = (T)ErrorObjectService.GetErrorObject(s.ModelType);
    //             error.Name = "Error";
    //             error.Description = "Exception: " + s.Exception.ToString();
    //             result.Add(error);
    //         }
    //         else
    //         {
    //             var model = await RestoreAble.Restore<T>(s.LoadedObject, restoreDebug);
    //             result.Add(model);
    //         }
    //     }
    //
    //     return result;
    // }

    // internal static async Task<List<Parameter>> GetParameters(string path, bool restoreDebug)
    // {
    //     var result = new List<Parameter>();
    //     var parameterStates = await PersistenceAPI.Instance.LoadRestoreAblesOfTypeAsync<RestoreState>(path, typeof(Parameter));
    //     parameterStates = parameterStates.OrderBy(p => p.LoadedObject?.Index).ToList();
    //     
    //     foreach (var p in parameterStates)
    //     {
    //         if (p.LoadedObject == null)
    //         {
    //             var parameter = new Parameter("Error", p.Exception.ToString());
    //             result.Add(parameter);
    //         }
    //         else
    //         {
    //             var parameter = await RestoreAble.Restore<Parameter>(p.LoadedObject, restoreDebug);
    //             result.Add(parameter);
    //         }
    //     }
    //     return result;
    // }
    
    // internal static async Task<List<UtilityContainerSelector>> GetUcs(string path, bool restoreDebug)
    // {
    //     var result = new List<UtilityContainerSelector>();
    //     var filter = FileExtensionService.GetFileExtensionFromType(typeof(UtilityContainerSelector));
    //     DebugService.Log("GetUcs path: " + path + " filter: " + filter, nameof(RestoreAbleService));
    //
    //     var states = await PersistenceAPI.Instance.LoadRestoreAblesAsync<RestoreState>(path, filter);
    //     DebugService.Log("GetUcs number of states: " + states.Count, nameof(RestoreAbleService));
    //
    //     states = states.OrderBy(s => s.LoadedObject?.Index).ToList();
    //     foreach (var bs in states)
    //     {
    //         if (bs.LoadedObject == null)
    //         {
    //             DebugService.Log("GetUcs failed fo load: " + bs.Path, nameof(RestoreAbleService));
    //             var error = (UtilityContainerSelector)AiObjectFactory.CreateInstance(bs.ModelType);
    //             var errorParam = new Parameter("Error", bs.Exception.ToString());
    //             error.AddParameter(errorParam);
    //             result.Add(error);
    //         }
    //         else
    //         {
    //             var ucs = await PersistSingleFile.Restore<UtilityContainerSelector>(bs.LoadedObject, restoreDebug);
    //             DebugService.Log("GetUcs loaded: " + ucs.GetName(), nameof(RestoreAbleService));
    //
    //             result.Add(ucs);
    //         }
    //     }
    //     return result;
    // }
    
    // internal static async Task LoadObjectsAndSortToCollection<T>(string path, List<string> namesOrdered, 
    //     ReactiveList<AiObjectModel> collection, bool restoreDebug) where T: AiObjectModel
    // {   
    //     DebugService.Log("Loading objects at: " + path, nameof(RestoreAbleService));
    //
    //     var aiObjects = await GetAiObjectsSortedByIndex<T>(path, restoreDebug);
    //     DebugService.Log("Loading objects at: " + path + " completed " + aiObjects.Count + " objects loaded", nameof(RestoreAbleService));
    //     if (aiObjects.Count > 0)
    //     {
    //         var sorted = OrderByNames(namesOrdered, aiObjects);
    //         collection.Add(sorted);
    //     }
    // }

    internal static async Task SaveRestoreAblesToFile<T>(IEnumerable<T> collection, string path, IPersister persister) where T: RestoreAble
    {
        var restoreAbles = collection.ToList();
        if (restoreAbles.Count == 0)
        {
            // Cleaning up to delete previously created files
            await PersistenceAPI.CleanUpAsync(path, DateTime.Now);
        }
        else
        {
            foreach (var element in restoreAbles)
            {
                if (string.IsNullOrEmpty(element.FileName) || string.IsNullOrWhiteSpace(element.FileName))
                {
                    throw new ArgumentException("A valid name must be set for element: " + element);
                }
                await element.SaveToFile(path,persister,restoreAbles.IndexOf(element));
            }
        }
    }
}
