using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

public abstract class AgentAction: AiObjectModel
{
    // public List<Parameter> Parameters;
    //private string namePostfix;

    protected AgentAction()
    {
        BaseAiObjectType = typeof(AgentAction);
    }

    public override string GetTypeDescription()
    {
        return "Agent Action";
    }
    protected override AiObjectModel InternalClone()
    {
        var clone = (AgentAction)AiObjectFactory.CreateInstance(GetType());
        return clone;
    }

    public virtual void OnStart(IAiContext context) { }
    public virtual void OnGoing(IAiContext context) { }
    public virtual void OnEnd(IAiContext context) { }

    protected override string GetNameFormat(string currentName)
    {
        return currentName;
    }

    internal override RestoreState GetState()
    {
        return new AgentActionState(Parameters.ToList(), Name, Description, this);
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        await base.RestoreInternalAsync(s, restoreDebug);
        var state = (AgentActionState)s;
        Name = state.Name;
        Description = state.Description;
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_AgentAction);
    }
}

[Serializable]
public class AgentActionState: AiObjectState
{
    public string Name;
    public string Description;
    public List<string> Parameters;

    public AgentActionState(): base()
    {
    }

    public AgentActionState(List<Parameter> parameters, string name, string description, AgentAction action): base(action)
    {
        Name = name;
        Description = description;
        Parameters = RestoreAbleService.NamesToList(parameters);

    }
}
