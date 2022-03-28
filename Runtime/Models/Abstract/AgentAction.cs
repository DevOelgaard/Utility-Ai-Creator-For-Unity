using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public abstract class AgentAction: AiObjectModel
{
    public List<Parameter> Parameters;
    private string namePostfix;

    protected AgentAction()
    {
        Parameters = new List<Parameter>(GetParameters());
        namePostfix = " (" + TypeDescriptor.GetClassName(this) + ")";
    }

    protected abstract List<Parameter> GetParameters();

    public virtual void OnStart(AiContext context) { }
    public virtual void OnGoing(AiContext context) { }
    public virtual void OnEnd(AiContext context) { }

    public override string GetNameFormat(string name)
    {
        return name;
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<AgentAction>(state);
        return clone;
    }

    internal override RestoreState GetState()
    {
        return new AgentActionState(Parameters, Name, Description, this);
    }

    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        var state = (AgentActionState)s;
        Name = state.Name;
        Description = state.Description;
        Parameters = new List<Parameter>();
        foreach (var p in state.Parameters)
        {
            var parameter = Parameter.Restore<Parameter>(p, restoreDebug);
            Parameters.Add(parameter);
        }
        if (this.GetType() == typeof(Demo_DebugLogParameter))
        {
            var p = Parameters.FirstOrDefault(p => p.Name == "Only OnGoing");
            if (p == null)
            {
                Parameters.Add(new Parameter("Only OnGoing", true));
            }
        }
        
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }
}

[Serializable]
public class AgentActionState: RestoreState
{
    public List<ParameterState> Parameters;
    public string Name;
    public string Description;

    public AgentActionState(): base()
    {
    }

    public AgentActionState(List<Parameter> parameters, string name, string description, AgentAction action): base(action)
    {
        Name = name;
        Description = description;

        Parameters = new List<ParameterState>();
        foreach (var parameter in parameters)
        {
            Parameters.Add(parameter.GetState() as ParameterState);
        }
    }
}
