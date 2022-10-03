using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;


[Serializable]
public class AgentModel
{
    private string name;
    public string Name {
        get => name; 
        set
        {
            name = value;
            onNameChanged.OnNext(name);
        }
    }

    public bool AutoTick = true;
    public int MsBetweenTicks = 0;
    public int FramesBetweenTicks = 0;
    internal float LastTickTime = 0f;
    internal int LastTickFrame = 0;

    public IObservable<string> OnNameChanged => onNameChanged;
    private Subject<string> onNameChanged = new Subject<string> ();
    public TickMetaData LastTickMetaData;
}
