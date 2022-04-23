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
    private static List<Ai> _ais = new List<Ai>();

    public static PlayAbleAiService Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new PlayAbleAiService();
                _instance.Init();
            }
            return _instance;
        }
    }

    [InitializeOnLoadMethod]
    private static void TouchInstance()
    {
        DebugService.Log("Initialize On Load", nameof(PlayAbleAiService));
        var x = Instance;
        DebugService.Log("Initialize On Load Complete", nameof(PlayAbleAiService));
    }

    private void Init()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            DebugService.Log("Initializing playmode", this);

            AsyncHelpers.RunSync(RestoreService);
        
            DebugService.Log("Initialized playmode complete Ais Count: " + _ais.Count, this);
            if (_ais.Count == 0)
            {
                DebugService.LogWarning("No Playable Ais", this);
            }
        }
        else
        {
            DebugService.Log("Initializing editor mode", this);

            if (TemplateService.Instance.isLoaded)
            {
                DebugService.Log("Loading Ais from TemplateService", this);
                UpdateAisFromTemplateService();                
                DebugService.Log("Loading Ais from TemplateService Complete", this);

            }
            else
            {
                DebugService.Log("Loading Ais from file", this);
                AsyncHelpers.RunSync(RestoreService);
                DebugService.Log("Loading Ais from file Complete", this);
            }

            TemplateService.Instance
                .AIs
                .OnValueChanged
                .Subscribe(_ => UpdateAisFromTemplateService())
                .AddTo(Disposables);
        
            DebugService.Log("Initialized editor mode complete Ais Count: " + _ais.Count, this);
        }

        EditorApplication.playModeStateChanged += Reset;
        // Application.logMessageReceived += Reset;
    }

    private void Reset(PlayModeStateChange obj)
    {
        DebugService.Log("Resetting because of playmode state change: " + obj, this);
        ClearSubscriptions();
        Init();
    }
    //
    // private void Reset(string logString, string stacktrace, LogType type)
    // {
    //     if (type != LogType.Exception) return;
    //     if (oldLogString == logString)
    //     {
    //         return;
    //     }
    //
    //     oldLogString = logString;
    //     DebugService.Log("Resetting because of exception: " + stacktrace, this);
    //     ClearSubscriptions();
    //     Init();
    // }



    private static PlayAbleAiService _instance;
    public static IObservable<List<Ai>> OnAisChanged => onAisChanged;
    // ReSharper disable once InconsistentNaming
    private static readonly Subject<List<Ai>> onAisChanged = new Subject<List<Ai>>();

    public Ai GetAiByName(string name)
    {
        if (_ais.Count == 0)
        {
            DebugService.LogWarning("No playable Ais. Have you marked an ai PlayAble", this);
            return null;
        }
        var ai = _ais.FirstOrDefault(ai => ai.Name == name) ?? _ais.First();
        DebugService.Log("GetAiByName requested name: " + name +" returning: " + ai.Name,this);
        return ai.Clone() as Ai;
    }

    public static IEnumerable<Ai> GetAis()
    {
        return _ais;
    }

    private PlayAbleAiService()
    {

    }
    private async Task RestoreService()
    {
        DebugService.Log("Restoring", this);
        var objectMetaData = await PersistenceAPI.Instance
            .LoadObjectPathAsync<PlayAbleAiServiceState>(Consts.FileUasPlayAblePathWithNameAndExtension);
        if (objectMetaData.IsSuccessFullyLoaded)
        {
            var state = objectMetaData.LoadedObject;
            await RestoreInternalAsync(state);
            //
            // _ais = new List<Ai>();
            // var tasks = state.AiStates
            //     .Select(aiState => 
            //         Task.Run(async () => 
            //             await Restore<Ai>(aiState)))
            //     .ToList();
            //
            // var results = await Task.WhenAll(tasks);
            // _ais = results.ToList();
            DebugService.Log("Restoring Complete", this);
        }
        else
        {
            DebugService.LogError("Failed to load playable service", this);
        }
    }

    private static void UpdateAisFromTemplateService()
    {
        _ais = new List<Ai>();
        AiDisposables.Clear();
        foreach (var ai in TemplateService.Instance.AIs.Values
                     .Cast<Ai>())
        {
            if (ai.IsPLayAble)
            {
                _ais.Add(ai);
            }
            ai.OnIsPlayableChanged
                .Subscribe(_ => UpdateAisFromTemplateService())
                .AddTo(AiDisposables);
        }
        DebugService.Log("Ais Updated Count: " + _ais.Count, Instance);
        Instance.SaveState();

        onAisChanged.OnNext(_ais);
    }
    protected override string GetFileName()
    {
        return Consts.FileUasPlayAbleFileName;
    }

    
    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        DebugService.Log("Restoring", this);
        var state = (PlayAbleAiServiceState)s;
        var directoryPath = Consts.FileUasPlayAblePath + Consts.FolderName_Ais;
        _ais = await RestoreAbleService.GetAiObjectsSortedByIndex<Ai>(directoryPath, restoreDebug);

        // var state = (PlayAbleAiServiceState) s;
        // _ais = new List<Ai>();
        // var tasks = state.AiStates
        //     .Select(aiState => 
        //         Task.Run(async () => 
        //             await Restore<Ai>(aiState)))
        //     .ToList();
        //
        // var results = await Task.WhenAll(tasks);
        // _ais = results.ToList();
        DebugService.Log("Restoring Complete Ai count: " + _ais.Count, this);

    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        DebugService.Log("Saving", this);
        var directoryPath = Path.GetDirectoryName(path);
        if (!path.Contains(Consts.FileExtension_UasPlayAble))
        {
            path += "." + Consts.FileExtension_UasPlayAble;
        }
        await persister.SaveObjectAsync(state, path);
        
        
        DebugService.Log("Starting save AI tasks", this);
        await RestoreAbleService.SaveRestoreAblesToFile(_ais,
            directoryPath + "/" + Consts.FolderName_Ais, persister);
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
        await PersistenceAPI.Instance.SaveObjectDestructivelyAsync(this, Consts.FileUasPlayAblePath,
            Consts.FileUasPlayAblePathWithNameAndExtension);
        DebugService.Log("Saving Complete ais count: " + _ais.Count, this);

    }
    private void ClearSubscriptions()
    {
        Disposables.Clear();
        AiDisposables.Clear();
        // Application.logMessageReceived -= Reset;
        EditorApplication.playModeStateChanged -= Reset;
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

    public PlayAbleAiServiceState(IEnumerable<Ai> ais, PlayAbleAiService o): base(o)
    {
        // AiStates = new List<AiState>();
        // foreach (var state in ais.Select(ai => 
        //              ai.GetState() as AiState))
        // {
        //     AiStates.Add(state);
        // }
    }
}

