using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

public class UaiTickerSettingsModel: RestoreAble
{
    private TickerMode currentTickerMode;
    internal TickerMode CurrentTickerMode
    {
        get => currentTickerMode ??= new TickerModeUnrestricted();
        set
        {
            currentTickerMode = value;
            onTickerModeChanged.OnNext(currentTickerMode);
        }
    }
    internal IObservable<TickerMode> OnTickerModeChanged => onTickerModeChanged;
    private readonly Subject<TickerMode> onTickerModeChanged = new Subject<TickerMode>();
    public bool AutoRun = true;

    internal List<TickerMode> TickerModes;

    protected override string GetFileName()
    {
        return Consts.FileName_TickerSettings;
    }

    protected override async Task RestoreInternalAsync(RestoreState state, bool restoreDebug = false)
    {
        var s = state as UaiTickerSettingsModelState;
        TickerModes = new List<TickerMode>();
        var modeStates = await PersistenceAPI.Instance
            .LoadObjectsOfTypeAsync<TickerModeState>(Consts.FolderPath_TickerModes_Complete, typeof(TickerMode));
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

        CurrentTickerMode = TickerModes.FirstOrDefault(tm => tm.Name == s.CurrentTickerMode);
        // CurrentTickerMode = await Restore<TickerMode>(s.Cu);
    }
    internal override RestoreState GetState()
    {
        return new UaiTickerSettingsModelState(CurrentTickerMode, TickerModes, this);
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, Consts.FilePath_TickerSettingsWithExtention);
        await RestoreAbleService.SaveRestoreAblesToFile(TickerModes,Consts.FolderPath_TickerModes_Complete, persister);
    }
}

[Serializable]
public class UaiTickerSettingsModelState: RestoreState
{
    // public TickerModeState TickerMode;
    public UaiTickerMode CurrentTickerMode;

    public UaiTickerSettingsModelState()
    {
    }

    public UaiTickerSettingsModelState(TickerMode tickerMode, List<TickerMode> tickerModes, UaiTickerSettingsModel o) : base(o)
    {
        CurrentTickerMode = tickerMode.Name;
        // TickerMode = tickerMode.GetState() as TickerModeState;
    }
}


