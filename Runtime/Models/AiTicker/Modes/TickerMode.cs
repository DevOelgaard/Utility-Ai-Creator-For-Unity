using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class TickerMode: RestoreAble
{
    internal AiTickerMode Name;
    internal string Description;
    internal List<Parameter> Parameters;

    protected TickerMode(AiTickerMode name, string description)
    {
        Description = description;
        Name = name;
        Parameters = GetParameters();
    }



    internal abstract List<Parameter> GetParameters();
    internal abstract void Tick(List<IAgent> agents, TickMetaData metaData);
    internal virtual void Tick(IAgent agent, TickMetaData metaData)
    {
        agent.Tick(metaData);
    }

    internal override RestoreState GetState()
    {
        return new TickerModeState(Name, Description, Parameters, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }

    protected override void RestoreInternal(RestoreState state, bool restoreDebug = false)
    {
        var s = state as TickerModeState;
        Name = Enum.Parse<AiTickerMode>(s.Name);
        Description = s.Description;
        Parameters = new List<Parameter>();
        foreach (var p in s.Parameters)
        {
            var parameter = Restore<Parameter>(p, restoreDebug);
            Parameters.Add(parameter);  
        }
    }
}

[Serializable]
public class TickerModeState: RestoreState
{
    public string Name;
    public string Description;
    public List<ParameterState> Parameters;

    public TickerModeState()
    {
    }

    public TickerModeState(AiTickerMode name, string description, List<Parameter> parameters, TickerMode o) : base(o)
    {
        Name = name.ToString();
        Description = description;
        Parameters = new List<ParameterState>();
        foreach(var parameter in parameters)
        {
            var pS = parameter.GetState() as ParameterState;
            Parameters.Add(pS);
        }
    }
}
