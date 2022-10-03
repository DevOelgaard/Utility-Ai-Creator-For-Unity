﻿using System;
using UnityEngine;
using Unity.EditorCoroutines.Editor;
using UniRx;

internal class MainWindowService
{
    private static MainWindowService instance;
    // private static Dictionary<Type, Queue<AiObjectComponent>> componentsByType = new Dictionary<Type, Queue<AiObjectComponent>>();
    // private List<KeyValuePair<Type,int>> typeAndInitAmountList = new List<KeyValuePair<Type, int>>()
    // {
    //     //new KeyValuePair<Type,int>(typeof(Ai),4),
    //     //new KeyValuePair<Type,int>(typeof(Bucket),5),
    //     //new KeyValuePair<Type,int>(typeof(Decision),10),
    //     //new KeyValuePair<Type,int>(typeof(Consideration),0),
    //     //new KeyValuePair<Type,int>(typeof(AgentAction),5),
    //     //new KeyValuePair<Type,int>(typeof(ResponseCurve),5),
    //     //new KeyValuePair<Type,int>(typeof(Demo_ConsiderationFixedValue),5),
    //     //new KeyValuePair<Type,int>(typeof(Demo_ConsiderationRandomValue),5),
    //     //new KeyValuePair<Type,int>(typeof(Demo_ConsiderationRandomValue),5),
    // };

    // private static Queue<MainWindowFoldedComponent> mainFoldedComponentPool = new Queue<MainWindowFoldedComponent>();
    // private const int poolSize = 48;
    // private const int mwfcPoolSize = 100;
    // private bool updatingPool = false;
    private EditorCoroutine updateComponentsCoroutine;
    internal IObservable<bool> OnUpdateStateChanged => onUpdateStateChanged;
    private readonly Subject<bool> onUpdateStateChanged = new Subject<bool>();
    public MainWindowService()
    {
    }

    internal void Start()
    {
        // Init();
        //EditorCoroutineUtility.StartCoroutine(Init(),this);
    }

    // private void Init()
    // {
    //     EditorCoroutineUtility.StartCoroutine(UpdatePoolsCoroutine(), this);
    // }
    //
    // private IEnumerator UpdatePoolsCoroutine()
    // {
    //     onUpdateStateChanged.OnNext(true);
    //     updatingPool = true;
    //     foreach (var kv in typeAndInitAmountList)
    //     {
    //         if (!componentsByType.ContainsKey(kv.Key))
    //         {
    //             componentsByType.Add(kv.Key, new Queue<AiObjectComponent>());
    //         }
    //         var queue = componentsByType[kv.Key];
    //         if (queue.Count < 5)
    //         {
    //             for (var i = 0; i < kv.Value; i++)
    //             {
    //                 var component = GetComponent(kv.Key);
    //                 queue.Enqueue(component);
    //                 yield return null;
    //             }
    //         }
    //     }
    //     if (mainFoldedComponentPool.Count < 5)
    //     {
    //         for (var i = 0; i < poolSize; i++)
    //         {
    //             mainFoldedComponentPool.Enqueue(new MainWindowFoldedComponent());
    //             yield return null;
    //         }
    //     }
    //     updatingPool = false;
    //     onUpdateStateChanged.OnNext(false);
    // }
    //
    internal MainWindowFoldedViewModel GetMainWindowFoldedComponent()
    {
        return new MainWindowFoldedViewModel();
        // if (mainFoldedComponentPool.Count == 0)
        // {
        //     if (!updatingPool)
        //     {
        //         updatingPool = true;
        //         EditorCoroutineUtility.StartCoroutine(UpdatePoolsCoroutine(), this);
        //     }
        //     return new MainWindowFoldedComponent();
        // } else if (mainFoldedComponentPool.Count < 5)
        // {
        //     if (!updatingPool)
        //     {
        //         updatingPool = true;
        //         EditorCoroutineUtility.StartCoroutine(UpdatePoolsCoroutine(), this);
        //     }
        // }
        // return mainFoldedComponentPool.Dequeue();
    }

    // internal MainWindowFoldedComponent RentMainWindowFoldedComponent()
    // {
    //     if (mainFoldedComponentPool.Count == 0)
    //     {
    //         if (!updatingPool)
    //         {
    //             updatingPool = true;
    //             EditorCoroutineUtility.StartCoroutine(UpdatePoolsCoroutine(), this);
    //         }
    //         return new MainWindowFoldedComponent();
    //     } else if (mainFoldedComponentPool.Count < 5)
    //     {
    //         if (!updatingPool)
    //         {
    //             updatingPool = true;
    //             EditorCoroutineUtility.StartCoroutine(UpdatePoolsCoroutine(), this);
    //         }
    //     }
    //     return mainFoldedComponentPool.Dequeue();
    // }

    internal AiObjectViewModel GetAiObjectComponent(AiObjectModel model)
    {
        var type = model.GetType();
        return GetAiObjectComponent(type);
    }
    
    internal AiObjectViewModel GetAiObjectComponent(Type modelType)
    {
        return GetComponent(modelType);
    }
    

    // internal AiObjectComponent RentComponent(Type type)
    // {
    //
    //     if (!componentsByType.ContainsKey(type))
    //     {
    //         if (!updatingPool)
    //         {
    //             updatingPool = true;
    //             //EditorCoroutineUtility.StartCoroutine(UpdatePoolsCoroutine(), this);
    //         }
    //
    //         return GetComponent(type);
    //     }
    //
    //     if (componentsByType[type].Count <= 0)
    //     {
    //         if (!updatingPool)
    //         {
    //             updatingPool = true;
    //             //EditorCoroutineUtility.StartCoroutine(UpdatePoolsCoroutine(), this);
    //         }
    //
    //         return GetComponent(type);
    //     }
    //     return componentsByType[type].Dequeue();
    // }
    //
    // internal void ReturnComponent(AiObjectComponent component)
    // {
    //     var type = component.Model.GetType();
    //     if (!componentsByType.ContainsKey(type))
    //     {
    //         componentsByType.Add(type, new Queue<AiObjectComponent>());
    //     }
    //     componentsByType[type].Enqueue(component);
    // }

    // internal void ReturnMWFC(MainWindowFoldedComponent component)
    // {
    //     mainFoldedComponentPool.Enqueue(component);
    // }
    private static ResponseCurveMainWindowViewModel _responseCurveWindow;

    private AiObjectViewModel GetComponent(Type type)
    {
        if (type == typeof(Uai) || type.IsSubclassOf(typeof(Uai)))
        {
            return new AiViewModel();
        }
        else if (type == typeof(Bucket) || type.IsSubclassOf(typeof(Bucket)))
        {
            return new BucketViewModel();
        }
        else if (type == typeof(Decision) || type.IsSubclassOf(typeof(Decision)))
        {
            return new DecisionViewModel();
        }
        else if (type == typeof(Consideration) || type.IsSubclassOf(typeof(Consideration)))
        {
            return new ConsiderationViewModel();
        }
        else if (type == typeof(AgentAction) || type.IsSubclassOf(typeof(AgentAction)))
        {
            return new AgentActionViewModel();
        }
        else if (type == typeof(ResponseCurve) || type.IsSubclassOf(typeof(ResponseCurve)))
        {
            return new ResponseCurveMainWindowViewModel();
        }
        Debug.LogError(type.ToString());
        throw new NotImplementedException();
    }


    internal Type GetTypeFromString(string label)
    {
        switch (label)
        {
            case Consts.Label_UAIModel:
                return typeof(Uai);
            case Consts.Label_BucketModel:
                return typeof(Bucket);
            case Consts.Label_DecisionModel:
                return typeof(Decision);
            case Consts.Label_ConsiderationModel:
                return typeof(Consideration);
            case Consts.Label_AgentActionModel:
                return typeof(AgentAction);
            case Consts.Label_ResponseCurve:
                return typeof(ResponseCurve);
            default:
                break;
        }
        return null;
    }

    internal static MainWindowService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new MainWindowService();
            }
            return instance;
        }
    }
    //
    // internal void PreloadComponents(ReactiveList<AiObjectModel> models)
    // {
    //     //throw new NotImplementedException();
    // }
}