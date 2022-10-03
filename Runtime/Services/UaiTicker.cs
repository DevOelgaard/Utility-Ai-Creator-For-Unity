using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEditor;
using Debug = UnityEngine.Debug;

internal class UaiTicker
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private IDisposable tickUntilTargetTickSub;

    private static UaiTicker _instance;
    public static UaiTicker Instance => _instance ??= new UaiTicker();
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
    
    internal UaiTickerSettingsModel Settings = new UaiTickerSettingsModel();
    private PersistenceAPI persistenceAPI => PersistenceAPI.Instance;

    private UaiTicker()
    {
        AsyncHelpers.RunSync(Init);

        EditorApplication.playModeStateChanged += HandlePLayModeStateChange;

        if (Debug.isDebugBuild)
        {
            if (Settings.AutoRun)
            {
                Start();
            }
        }
    }

    private void HandlePLayModeStateChange(PlayModeStateChange playModeChange)
    {
        if (playModeChange == PlayModeStateChange.ExitingEditMode)
        {
            DebugService.Log("Handling playMode: " + playModeChange, this);

            AsyncHelpers.RunSync(SaveSettings);
        }
    }

    private async Task Init()
    {
        var loadedSettings = await persistenceAPI
            .LoadObjectPathAsync<UaiTickerSettingsModelSingleFileState>(Consts.FilePath_TickerSettingsWithExtention);
        
        if (loadedSettings.LoadedObject != null)
        {
            Settings = await PersistSingleFile.Restore<UaiTickerSettingsModel>(loadedSettings.LoadedObject);
        } else
        {
            DebugService.LogWarning("Failed to load AiTicker settings Error: " + loadedSettings.ErrorMessage + " Exception: " + loadedSettings.Exception, this);
            InitSettings();
        }
    }
    
    public void Start()
    {
        Observable.IntervalFrame(1)
            .Subscribe(_ => TickAis())
            .AddTo(disposables);
    }

    public void Stop()
    {
        disposables.Clear();
    }

    public void TickAgent(IAgent agent)
    {
        if (!EditorApplication.isPlaying) return;
        if (EditorApplication.isPaused) return;
        TickCount++;
        var metaData = new TickMetaData
        {
            TickCount = TickCount
        };
        Settings.CurrentTickerMode.Tick(agent, metaData);
        onTickComplete.OnNext(TickCount);
    }

    public void TickUntilCount(int targetTickCount, bool pauseOnComplete)
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

    
    public void TickAis()
    {
        if (!EditorApplication.isPlaying) return;
        if (EditorApplication.isPaused) return;
        TickCount++;
        var metaData = new TickMetaData
        {
            TickCount = TickCount
        };
        Settings.CurrentTickerMode
            .Tick(agentManager
                .Model
                .Agents
                .Values
                .Where(agent => agent.CanAutoTick())
                .ToList(), metaData);

        onTickComplete.OnNext(TickCount);
    }

    internal void SetTickerMode(UaiTickerMode tickerMode)
    {
        var newMode = Settings.TickerModes.FirstOrDefault(m => m.Name == tickerMode);
        if (newMode == null) return;
        Settings.CurrentTickerMode = newMode;
    }

    internal void InitSettings()
    {
        Settings.TickerModes = new List<TickerMode>
        {
            new TickerModeDesiredFrameRate(),
            new TickerModeTimeBudget(),
            new TickerModeUnrestricted()
        };

        Settings.CurrentTickerMode = Settings.TickerModes.First(m => m.Name == UaiTickerMode.Unrestricted);
    }
    
    private async Task SaveSettings()
    {
        DebugService.Log("Saving",this);
        await Settings.SaveToFile(Consts.FilePath_TickerSettingsWithExtention);
        DebugService.Log("Saving Complete",this);
    }
    
    ~UaiTicker()
    {
        DebugService.Log("Destroying",this);
        disposables.Clear();
        EditorApplication.playModeStateChanged -= HandlePLayModeStateChange;
        DebugService.Log("Destroying Complete",this);

    }
}

