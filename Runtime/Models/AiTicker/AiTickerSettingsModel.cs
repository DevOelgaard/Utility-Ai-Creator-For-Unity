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



    protected override void RestoreInternal(RestoreState state, bool restoreDebug = false)
    {
        var s = state as AiTickerSettingsState;
        TickerMode = Restore<TickerMode>(s.TickerMode);
        TickerModes = new List<TickerMode>();
        foreach(var t in s.TickerModes)
        {
            var tickerM = Restore<TickerMode>(t, restoreDebug);
            TickerModes.Add(tickerM);
        }
    }
    internal override RestoreState GetState()
    {
        return new AiTickerSettingsState(TickerMode, TickerModes, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }
}

[Serializable]
public class AiTickerSettingsState: RestoreState
{
    public TickerModeState TickerMode;
    public List<TickerModeState> TickerModes;

    public AiTickerSettingsState()
    {
    }

    public AiTickerSettingsState(TickerMode tickerMode, List<TickerMode> tickerModes, AiTickerSettingsModel o) : base(o)
    {
        TickerMode = tickerMode.GetState() as TickerModeState;
        TickerModes = new List<TickerModeState>();
        foreach(var tickerM in tickerModes)
        {
            var t = tickerM.GetState() as TickerModeState;
            TickerModes.Add(t);
        }
    }
}


