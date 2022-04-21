using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UniRx;
using UnityEngine;

public class PlayAbleAiService: RestoreAble
{
    private static readonly CompositeDisposable Disposables = new CompositeDisposable();
    private static readonly CompositeDisposable AiDisposables = new CompositeDisposable();
    private static List<Ai> _ais = new List<Ai>();
    private static bool _saveOnDestroy = true;

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
        var x = Instance;
    }

    private void Init()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode)
        {
            Debug.Log("PlayAbleService: Initializing playmode");

            _saveOnDestroy = false;
            AsyncHelpers.RunSync(RestoreService);
        
            Debug.Log("PlayAbleService: Initialized playmode complete");
            if (_ais.Count == 0)
            {
                Debug.LogWarning("No Playable Ais");
            }
        }
        else
        {
            Debug.Log("PlayAbleService: Initializing editor mode");

            EditorApplication.playModeStateChanged += Instance.SaveIfExitingEditorMode;
        
            Debug.Log("PlayAbleService: Initialized editor mode complete");
        }
    }

    private static PlayAbleAiService _instance;
    public static IObservable<List<Ai>> OnAisChanged => onAisChanged;
    // ReSharper disable once InconsistentNaming
    private static readonly Subject<List<Ai>> onAisChanged = new Subject<List<Ai>>();

    public Ai GetAiByName(string name)
    {
        return _ais.FirstOrDefault(ai => ai.Name == name)?.Clone() as Ai;
    }

    public static IEnumerable<Ai> GetAis()
    {
        return _ais;
    }

    private PlayAbleAiService()
    {
        UasTemplateService.Instance
            .AIs
            .OnValueChanged
            .Subscribe(_ => UpdateAis(!EditorApplication.isPlayingOrWillChangePlaymode))
            .AddTo(Disposables);
    }

    private async Task RestoreService()
    {
        Debug.Log("PlayAbleService: Restoring");
        var objectMetaData = PersistenceAPI.Instance.LoadObjectPath<PlayAbleAiServiceState>(Consts.FileUasPlayAbleWithExtension);
        if (objectMetaData.Success)
        {

            var state = objectMetaData.LoadedObject;
            _ais = new List<Ai>();
            var tasks = state.AiStates
                .Select(aiState => 
                    Task.Run(async () => 
                        await Restore<Ai>(aiState)))
                .ToList();

            var results = await Task.WhenAll(tasks);
            _ais = results.ToList();
            Debug.Log("PlayAbleService: Restoring Complete");
        }
        else
        {
            Debug.LogError("Failed to load playable service");
        }
    }

    private static void UpdateAis(bool addAis)
    {
        _ais = new List<Ai>();
        AiDisposables.Clear();
        foreach (var ai in UasTemplateService.Instance.AIs.Values
                     .Cast<Ai>())
        {
            if (addAis && ai.IsPLayAble)
            {
                _ais.Add(ai);
            }
            ai.OnIsPlayableChanged
                .Subscribe(_ => UpdateAis(true))
                .AddTo(AiDisposables);
        }
        Debug.Log("Ais Updated Count: " + _ais.Count);

        onAisChanged.OnNext(_ais);
    }
    protected override string GetFileName()
    {
        return "Play Able Ais";
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        Debug.Log("PlayAbleService: Restoring");

        var state = (PlayAbleAiServiceState) s;
        _ais = new List<Ai>();
        var tasks = state.AiStates
            .Select(aiState => 
                Task.Run(async () => 
                    await Restore<Ai>(aiState)))
            .ToList();

        var results = await Task.WhenAll(tasks);
        _ais = results.ToList();
        Debug.Log("PlayAbleService: Restoring Complete");

    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        Debug.Log("PlayAbleService: Saving");
        if (!path.Contains(Consts.FileExtension_UasPlayAble))
        {
            path += "." + Consts.FileExtension_UasPlayAble;
        }
        await persister.SaveObjectAsync(state, path);
        Debug.Log("PlayAbleService: Saving Complete");
    }

    internal override RestoreState GetState()
    {
        return new PlayAbleAiServiceState(_ais,this);
    }

    private void SaveIfExitingEditorMode(PlayModeStateChange playModeState)
    {
        Debug.Log("PlayAbleService: SaveIfExitingEditorMode playModeState: " + playModeState);

        if (playModeState != PlayModeStateChange.ExitingEditMode) return;
        if (!_saveOnDestroy) return;
        Debug.Log("PlayAbleService: Saving");

        var state = GetState();
        PersistenceAPI.Instance
            .SaveDestructiveObjectPath(state, Consts.FileUasPlayAbleWithExtension);
        Debug.Log("PlayAbleService: Saving Complete");
    }

    ~PlayAbleAiService()
    {
        Disposables.Clear();
        AiDisposables.Clear();
        EditorApplication.playModeStateChanged -= Instance.SaveIfExitingEditorMode;
    }
}

[Serializable]
public class PlayAbleAiServiceState: RestoreState
{
    public List<AiState> AiStates;

    public PlayAbleAiServiceState()
    {
    }

    public PlayAbleAiServiceState(IEnumerable<Ai> ais, PlayAbleAiService o): base(o)
    {
        AiStates = new List<AiState>();
        foreach (var state in ais.Select(ai => 
                     ai.GetState() as AiState))
        {
            AiStates.Add(state);
        }
    }
}

