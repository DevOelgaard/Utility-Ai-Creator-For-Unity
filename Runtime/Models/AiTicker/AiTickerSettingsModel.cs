using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

public class AiTickerSettingsModel: RestoreAble
{
    private TickerMode tickerMode;
    internal TickerMode TickerMode
    {
        get => tickerMode;
        set
        {
            tickerMode = value;
            onTickerModeChanged.OnNext(tickerMode);
        }
    }
    internal IObservable<TickerMode> OnTickerModeChanged => onTickerModeChanged;
    private Subject<TickerMode> onTickerModeChanged = new Subject<TickerMode>();
    public bool AutoRun = true;

    internal List<TickerMode> TickerModes;

    protected override string GetFileName()
    {
        return "AiTickerSettings";
    }

    protected override async Task RestoreInternalAsync(RestoreState state, bool restoreDebug = false)
    {
        var task = Task.Factory.StartNew(() =>
        {
            var s = state as AiTickerSettingsState;
            TickerMode = Restore<TickerMode>(s.TickerMode);
            TickerModes = new List<TickerMode>();
            var modeStates = PersistenceAPI.Instance.LoadObjectsPathWithFilters<TickerModeState>(CurrentDirectory + Consts.FolderName_TickerModes, typeof(TickerMode));
            foreach(var mode in modeStates)
            {
                if(mode.LoadedObject == null)
                {
                    var tm = new TickerModeUnrestricted
                    {
                        Description = mode.ErrorMessage + "Exception: " + mode.Exception.ToString()
                    };
                }
                else
                {
                    var tickerModeLocal = Restore<TickerMode>(mode.LoadedObject, restoreDebug);
                    TickerModes.Add(tickerModeLocal);
                }
            }
        });
        await task;
    }
    internal override RestoreState GetState()
    {
        return new AiTickerSettingsState(TickerMode, TickerModes, this);
    }

    protected override void InternalSaveToFile(string path, IPersister destructivePersister, RestoreState state)
    {
        destructivePersister.SaveObject(state, path + Consts.FileExtension_TickerSettings);
        foreach(var mode in TickerModes)
        {
            var subPath = path + "/" + Consts.FolderName_TickerModes;
            mode.SaveToFile(subPath, destructivePersister);
        }
    }
}

[Serializable]
public class AiTickerSettingsState: RestoreState
{
    public TickerModeState TickerMode;

    public AiTickerSettingsState()
    {
    }

    public AiTickerSettingsState(TickerMode tickerMode, List<TickerMode> tickerModes, AiTickerSettingsModel o) : base(o)
    {
        TickerMode = tickerMode.GetState() as TickerModeState;
    }
}


