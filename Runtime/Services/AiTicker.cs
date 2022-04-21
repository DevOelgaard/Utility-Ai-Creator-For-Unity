using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEditor;
using Debug = UnityEngine.Debug;

internal class AiTicker: RestoreAble
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private IDisposable tickUntilTargetTickSub;

    private static AiTicker _instance;
    public static AiTicker Instance => _instance ??= new AiTicker();
    private AgentManager agentManager => AgentManager.Instance;

    public IObservable<int> OnTickComplete => onTickComplete;
    private readonly Subject<int> onTickComplete = new Subject<int>();

    private int tickCount; 
    public int TickCount { 
        get => tickCount; 
        private set
        {
            tickCount = value;
            onTickCountChanged.OnNext(tickCount);
        } 
    }

    public IObservable<int> OnTickCountChanged => onTickCountChanged;
    private readonly Subject<int> onTickCountChanged = new Subject<int>();
    
    internal AiTickerSettingsModel Settings = new AiTickerSettingsModel();
    private PersistenceAPI persistenceAPI => PersistenceAPI.Instance;

    private AiTicker()
    {
        AsyncHelpers.RunSync(Init);

        if (Debug.isDebugBuild)
        {
            if (Settings.AutoRun)
            {
                Start();
            }
        }
    }

    private async Task Init()
    {
        var loadedState = await persistenceAPI
            .LoadObjectPathAsync<AiTickerSettingsState>(Consts.FileTickerSettings);
        
        if (loadedState.LoadedObject != null)
        {
            Settings = await Restore<AiTickerSettingsModel>(loadedState.LoadedObject);
        } else
        {
            Debug.LogWarning("Failed to load AiTicker settings Error: " + loadedState.ErrorMessage + " Exception: " + loadedState.Exception);
            Reload();
        }
    }
    
    internal void Start()
    {
        Observable.IntervalFrame(1)
            .Subscribe(_ => TickAis())
            .AddTo(disposables);
    }

    internal void Stop()
    {
        disposables.Clear();
    }

    protected override string GetFileName()
    {
        return "AiTicker";
    }

    internal void TickAgent(IAgent agent)
    {
        if (!EditorApplication.isPlaying) return;
        if (EditorApplication.isPaused) return;
        TickCount++;
        var metaData = new TickMetaData
        {
            TickCount = TickCount
        };
        Settings.TickerMode.Tick(agent, metaData);
        onTickComplete.OnNext(TickCount);
    }

    internal void TickUntilCount(int targetTickCount, bool pauseOnComplete)
    {
        tickUntilTargetTickSub = OnTickComplete
            .Subscribe(completedTickCount =>
            {
                if (completedTickCount < targetTickCount) return;
                if (pauseOnComplete)
                {
                    EditorApplication.isPaused = true;
                }
                Stop();
                tickUntilTargetTickSub.Dispose();
            });
        Start();
        EditorApplication.isPaused = false;
        EditorApplication.isPlaying = true;
    }

    internal void TickAis()
    {
        if (!EditorApplication.isPlaying) return;
        if (EditorApplication.isPaused) return;
        TickCount++;
        var metaData = new TickMetaData
        {
            TickCount = TickCount
        };
        Settings.TickerMode
            .Tick(agentManager
                .Model
                .Agents
                .Values
                .Where(agent => agent.CanAutoTick())
                .ToList(), metaData);

        onTickComplete.OnNext(TickCount);
    }

    internal void SetTickerMode(AiTickerMode tickerMode)
    {
        var newMode = Settings.TickerModes.FirstOrDefault(m => m.Name == tickerMode);
        if (newMode == null) return;
        Settings.TickerMode = newMode;
    }

    protected override async Task RestoreInternalAsync(RestoreState state, bool restoreDebug = false)
    {
        var s = state as AiTickerState;
        // ReSharper disable once PossibleNullReferenceException
        Settings = await Restore<AiTickerSettingsModel>(s.settings, restoreDebug);
    }

    internal override RestoreState GetState()
    {   
        return new AiTickerState(Settings, this);
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_AiTicker);
    }

    internal void Reload()
    {
        Settings.TickerModes = new List<TickerMode>
        {
            new TickerModeDesiredFrameRate(),
            new TickerModeTimeBudget(),
            new TickerModeUnrestricted()
        };

        Settings.TickerMode = Settings.TickerModes.First(m => m.Name == AiTickerMode.Unrestricted);
    }

    internal async Task Save()
    {
        await persistenceAPI.SaveObjectPath(Settings, Consts.FileTickerSettings, "TickerSettings");
    }

    ~AiTicker()
    {
        AsyncHelpers.RunSync(Save);
        disposables.Clear();
    }
}

[Serializable]
public class AiTickerState: RestoreState
{
    public AiTickerSettingsState settings;

    public AiTickerState()
    {
    }

    internal AiTickerState(AiTickerSettingsModel settings, AiTicker o) : base(o)
    {
        this.settings = settings.GetState() as AiTickerSettingsState;
    }
}
