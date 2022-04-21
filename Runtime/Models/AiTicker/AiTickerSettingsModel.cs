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
    private readonly Subject<TickerMode> onTickerModeChanged = new Subject<TickerMode>();
    public bool AutoRun = true;

    internal List<TickerMode> TickerModes;

    protected override string GetFileName()
    {
        return "AiTickerSettings";
    }

    protected override async Task RestoreInternalAsync(RestoreState state, bool restoreDebug = false)
    {
        var s = state as AiTickerSettingsState;
        TickerMode = await Restore<TickerMode>(s.TickerMode);
        TickerModes = new List<TickerMode>();
        var modeStates = await PersistenceAPI.Instance
            .LoadObjectsPathWithFilters<TickerModeState>(CurrentDirectory + Consts.FolderName_TickerModes, typeof(TickerMode));
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
                var tickerModeLocal =  await Restore<TickerMode>(mode.LoadedObject, restoreDebug);
                TickerModes.Add(tickerModeLocal);
            }
        }
    }
    internal override RestoreState GetState()
    {
        return new AiTickerSettingsState(TickerMode, TickerModes, this);
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObject(state, path + Consts.FileExtension_TickerSettings);
        await RestoreAbleService.SaveRestoreAblesToFile(TickerModes,path + "/" + Consts.FolderName_TickerModes, persister);
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


