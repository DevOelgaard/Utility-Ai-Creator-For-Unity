﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NSubstitute.Exceptions;
using UnityEditor;
using UniRx;
using UnityEngine;

public class PlayAbleAiService: RestoreAble
{
    private static readonly CompositeDisposable Disposables = new CompositeDisposable();
    private static readonly CompositeDisposable AiDisposables = new CompositeDisposable();
    public List<Uai> PlayAbleAIs = new List<Uai>();

    public static PlayAbleAiService Instance => _instance ??= new PlayAbleAiService();
    private static PlayAbleAiService _instance;

    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        DebugService.Log("Initializing on load",nameof(PlayAbleAiService));
        
        if (!EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Instance.InitEditorMode();
        }
    }

    private PlayAbleAiService()
    {
        DebugService.Log("Subscribing to playModeStateChanged", nameof(PlayAbleAiService));
        EditorApplication.playModeStateChanged += HandlePlaymodeStateChange;
    }

    private static void HandlePlaymodeStateChange(PlayModeStateChange playModeState)
    {
        if (playModeState == PlayModeStateChange.EnteredEditMode)
        {
            Instance.InitEditorMode();
        } else if (playModeState == PlayModeStateChange.ExitingEditMode)
        {
            Instance.ExitingPlayMode();
        }
    }

    private void InitEditorMode()
    {
        DebugService.Log("Initializing editor mode", nameof(PlayAbleAiService));

        Instance.UpdateAisFromTemplateService(true);                

        TemplateService.Instance
            .AIs
            .OnValueChanged
            .Subscribe(_ => Instance.UpdateAisFromTemplateService(true))
            .AddTo(Disposables);
        
        DebugService.Log("Initialized editor mode complete Ais Count: " + Instance.PlayAbleAIs.Count, this);
    }

    [RuntimeInitializeOnLoadMethod]
    private static void InitPlayMode()
    {
        DebugService.Log("Initializing playmode", Instance);

        AsyncHelpers.RunSync(Instance.RestoreService);
        
        DebugService.Log("Initialized playmode complete Ais Count: " + Instance.PlayAbleAIs.Count, Instance);
        if (Instance.PlayAbleAIs.Count == 0)
        {
            DebugService.LogWarning("No Playable Ais", Instance);
        }
    }
    

    public IObservable<List<Uai>> OnAisChanged => onAisChanged;
    // ReSharper disable once InconsistentNaming
    private readonly Subject<List<Uai>> onAisChanged = new Subject<List<Uai>>();

    public Uai GetAiByName(string name)
    {
        if (PlayAbleAIs.Count == 0)
        {
            DebugService.LogWarning("No playable Ais. Have you marked an ai PlayAble", this);
            return null;
        }
        var ai = PlayAbleAIs.FirstOrDefault(ai => ai.Name == name) ?? PlayAbleAIs.First();
        DebugService.Log("GetAiByName requested name: " + name +" returning: " + ai.Name,this);
        return ai.Clone() as Uai;
    }

    private async Task RestoreService()
    {
        DebugService.Log("Restoring", this);
        var objectMetaData = await PersistenceAPI.Instance
            .LoadObjectPathAsync<PlayAbleAiServiceState>(Consts.FilePath_PlayAbleAiWithExtention);
        if (objectMetaData.IsSuccessFullyLoaded)
        {
            var state = objectMetaData.LoadedObject;
            await RestoreInternalAsync(state);
            DebugService.Log("Restoring Complete", this);
        }
        else
        {
            DebugService.LogError("Failed to load playable service. Error Message: " + objectMetaData?.ErrorMessage, this, objectMetaData?.Exception);
        }
    }

    internal void UpdateAisFromTemplateService(bool saveAfterUpdate)
    {
        PlayAbleAIs = new List<Uai>();
        AiDisposables.Clear();
        foreach (var ai in TemplateService.Instance.AIs.Values
                     .Cast<Uai>())
        {
            if (ai.IsPLayAble)
            {
                PlayAbleAIs.Add(ai);
            }
            ai.OnIsPlayableChanged
                .Subscribe(_ => UpdateAisFromTemplateService(true))
                .AddTo(AiDisposables);
        }
        DebugService.Log("Ais Updated Count: " + PlayAbleAIs.Count, this);

        if (saveAfterUpdate)
        {
            SaveState();
        }

        onAisChanged.OnNext(PlayAbleAIs);
    }
    
    private void ExitingPlayMode()
    {
        DebugService.Log("Exiting Editor Mode", this);
        SaveState();
        DebugService.Log("Exiting Editor Mode - Complete", this);
    }
    
    protected override string GetFileName()
    {
        return Consts.FileName_PLayAbleAi;
    }
    
    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        DebugService.Log("RestoringInternal", this);
        PlayAbleAIs = await RestoreAbleService.GetAiObjectsSortedByIndex<Uai>(Consts.Folder_PlayAbleAi_Complete, restoreDebug);

        DebugService.Log("RestoringInternal Complete Ai count: " + PlayAbleAIs.Count, this);
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        DebugService.Log("InternalSaveToFile", this);

        await persister.SaveObjectAsync(state, Consts.FilePath_PlayAbleAiWithExtention);
        
        
        await RestoreAbleService.SaveRestoreAblesToFile(PlayAbleAIs, Consts.Folder_PlayAbleAi_Complete, persister);
        DebugService.Log("InternalSaveToFile - Complete", this);
    }

    internal override RestoreState GetState()
    {
        return new PlayAbleAiServiceState(PlayAbleAIs,this);
    }

    private async void SaveState()
    {
        if (EditorApplication.isPlaying) return;
        DebugService.Log("Saving", this);
        await PersistenceAPI.Instance.SaveObjectDestructivelyAsync(this, Consts.FilePath_PlayAbleAiWithExtention);
        DebugService.Log("Saving - Complete ais count: " + PlayAbleAIs.Count, this);
    }
    private void ClearSubscriptions()
    {
        DebugService.Log("Clearing subscriptions", this);

        Disposables.Clear();
        AiDisposables.Clear();
        EditorApplication.playModeStateChanged -= HandlePlaymodeStateChange;
    }

    ~PlayAbleAiService()
    {
        DebugService.Log("Destroying", this);
        ClearSubscriptions();
        DebugService.Log("Destroying complete", this);
    }
}

[Serializable]
public class PlayAbleAiServiceState: RestoreState
{
    public PlayAbleAiServiceState()
    {
    }

    public PlayAbleAiServiceState(IEnumerable<Uai> ais, PlayAbleAiService o): base(o)
    {
    }
}

