using System;
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
    private static List<Uai> _ais = new List<Uai>();

    public static PlayAbleAiService Instance => _instance ??= new PlayAbleAiService();
    private static PlayAbleAiService _instance;

    [InitializeOnLoadMethod]
    private static void InitializeOnLoad()
    {
        if (_instance != null) return;
        DebugService.Log("Subscribing to playModeStateChanged", nameof(PlayAbleAiService));
        EditorApplication.playModeStateChanged += InitEditorMode;
    }

    private static void InitEditorMode(PlayModeStateChange playModeState)
    {
        DebugService.Log("Initializing editor mode", nameof(PlayAbleAiService));
        if (playModeState!= PlayModeStateChange.EnteredEditMode)
        {
            DebugService.Log("Initializing editor mode aborted not editor mode state:" + playModeState, nameof(PlayAbleAiService));
            return;
        };
        var instance = Instance;

        if (TemplateService.Instance.isLoaded)
        {
            DebugService.Log("Loading Ais from TemplateService", instance);
            UpdateAisFromTemplateService(false);                
            DebugService.Log("Loading Ais from TemplateService Complete", instance);

        }
        else
        {
            DebugService.Log("Loading Ais from file", instance);
            AsyncHelpers.RunSync(instance.RestoreService);
            DebugService.Log("Loading Ais from file Complete", instance);
        }

        TemplateService.Instance
            .AIs
            .OnValueChanged
            .Subscribe(_ => UpdateAisFromTemplateService(false))
            .AddTo(Disposables);
        
        DebugService.Log("Initialized editor mode complete Ais Count: " + _ais.Count, instance);
    }

    [RuntimeInitializeOnLoadMethod]
    private static void InitPlayMode()
    {
        var instance = Instance;
        DebugService.Log("Initializing playmode", instance);

        AsyncHelpers.RunSync(instance.RestoreService);
        
        DebugService.Log("Initialized playmode complete Ais Count: " + _ais.Count, instance);
        if (_ais.Count == 0)
        {
            DebugService.LogWarning("No Playable Ais", instance);
        }
    }

    public static IObservable<List<Uai>> OnAisChanged => onAisChanged;
    // ReSharper disable once InconsistentNaming
    private static readonly Subject<List<Uai>> onAisChanged = new Subject<List<Uai>>();

    public Uai GetAiByName(string name)
    {
        if (_ais.Count == 0)
        {
            DebugService.LogWarning("No playable Ais. Have you marked an ai PlayAble", this);
            return null;
        }
        var ai = _ais.FirstOrDefault(ai => ai.Name == name) ?? _ais.First();
        DebugService.Log("GetAiByName requested name: " + name +" returning: " + ai.Name,this);
        return ai.Clone() as Uai;
    }

    public static IEnumerable<Uai> GetAis()
    {
        return _ais;
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

    internal static void UpdateAisFromTemplateService(bool saveAfterUpdate)
    {
        _ais = new List<Uai>();
        AiDisposables.Clear();
        foreach (var ai in TemplateService.Instance.AIs.Values
                     .Cast<Uai>())
        {
            if (ai.IsPLayAble)
            {
                _ais.Add(ai);
            }
            ai.OnIsPlayableChanged
                .Subscribe(_ => UpdateAisFromTemplateService(false))
                .AddTo(AiDisposables);
        }
        DebugService.Log("Ais Updated Count: " + _ais.Count, Instance);

        if (saveAfterUpdate)
        {
            Instance.SaveState();
        }

        onAisChanged.OnNext(_ais);
    }
    protected override string GetFileName()
    {
        return Consts.FileName_PLayAbleAi;
    }
    
    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        DebugService.Log("RestoringInternal", this);
        // var state = (PlayAbleAiServiceState)s;
        // var directoryPath = Consts.FileUasPlayAblePath + Consts.FolderName_Ais;
        _ais = await RestoreAbleService.GetAiObjectsSortedByIndex<Uai>(Consts.Folder_PlayAbleAi_Complete, restoreDebug);

        DebugService.Log("RestoringInternal Complete Ai count: " + _ais.Count, this);
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        // DebugService.Log("Saving", this);
        // var directoryPath = Path.GetDirectoryName(path);
        // if (!path.Contains(Consts.FileExtension_PlayAble))
        // {
        //     path += "." + Consts.FileExtension_PlayAble;
        // }
        await persister.SaveObjectAsync(state, Consts.FilePath_PlayAbleAiWithExtention);
        
        
        DebugService.Log("Starting save AI tasks", this);
        await RestoreAbleService.SaveRestoreAblesToFile(_ais, Consts.Folder_PlayAbleAi_Complete, persister);
        DebugService.Log("Saving Complete", this);
    }

    internal override RestoreState GetState()
    {
        return new PlayAbleAiServiceState(_ais,this);
    }

    private async void SaveState()
    {
        DebugService.Log("Saving", this);
        if (EditorApplication.isPlaying) return;
        var state = GetState();
        await PersistenceAPI.Instance.SaveObjectDestructivelyAsync(this, Consts.FilePath_PlayAbleAiWithExtention);
        DebugService.Log("Saving Complete ais count: " + _ais.Count, this);

    }
    private void ClearSubscriptions()
    {
        DebugService.Log("Clearing subscriptions", this);

        Disposables.Clear();
        AiDisposables.Clear();
        EditorApplication.playModeStateChanged -= InitEditorMode;
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
    // public List<AiState> AiStates;

    public PlayAbleAiServiceState()
    {
    }

    public PlayAbleAiServiceState(IEnumerable<Uai> ais, PlayAbleAiService o): base(o)
    {
        // AiStates = new List<AiState>();
        // foreach (var state in ais.Select(ai => 
        //              ai.GetState() as AiState))
        // {
        //     AiStates.Add(state);
        // }
    }
}

