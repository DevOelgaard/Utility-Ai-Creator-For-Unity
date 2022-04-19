using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEditor;
using Debug = UnityEngine.Debug;

internal class AiTicker: RestoreAble
{
    private CompositeDisposable disposables = new CompositeDisposable();
    private IDisposable tickUntillTargetTickSub;

    private static AiTicker instance;
    public static AiTicker Instance => instance ??= new AiTicker();
    private AgentManager agentManager => AgentManager.Instance;

    public IObservable<int> OnTickComplete => onTickComplete;
    private Subject<int> onTickComplete = new Subject<int>();

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
        Init();

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
            Debug.LogWarning("Failed to load AiTicker settings Error: " + loadedState.ErrorMessage + " Exception: " + loadedState.Exception?.ToString());
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
        var metaData = new TickMetaData();
        metaData.TickCount = TickCount;
        Settings.TickerMode.Tick(agent, metaData);
        onTickComplete.OnNext(TickCount);
    }

    internal void TickUntilCount(int targetTickCount, bool pauseOnComplete)
    {
        tickUntillTargetTickSub = OnTickComplete
            .Subscribe(tickCount =>
            {
                if (tickCount >= targetTickCount)
                {
                    if (pauseOnComplete)
                    {
                        EditorApplication.isPaused = true;
                    }
                    Stop();
                    tickUntillTargetTickSub.Dispose();
                }
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
        Settings = await Restore<AiTickerSettingsModel>(s.Settings, restoreDebug);
    }

    internal override RestoreState GetState()
    {
        return new AiTickerState(Settings, this);
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObject(state, path + "." + Consts.FileExtension_AiTicker);
    }

    internal void Reload()
    {
        Settings.TickerModes = new List<TickerMode>();
        Settings.TickerModes.Add(new TickerModeDesiredFrameRate());
        Settings.TickerModes.Add(new TickerModeTimeBudget());
        Settings.TickerModes.Add(new TickerModeUnrestricted());

        Settings.TickerMode = Settings.TickerModes.First(m => m.Name == AiTickerMode.Unrestricted);
    }

    internal async Task Save()
    {
        await persistenceAPI.SaveObjectPath(Settings, Consts.FileTickerSettings, "TickerSettings");
    }

    ~AiTicker()
    {
        Save();
        disposables.Clear();
    }
}



[Serializable]
public class AiTickerState: RestoreState
{
    public AiTickerSettingsState Settings;

    public AiTickerState()
    {
    }

    internal AiTickerState(AiTickerSettingsModel settings, AiTicker o) : base(o)
    {
        Settings = settings.GetState() as AiTickerSettingsState;
    }
}
