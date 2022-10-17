using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

public abstract class AgentAction: AiObjectModel
{
    protected AgentAction()
    {
        BaseAiObjectType = typeof(AgentAction);
    }


    protected override AiObjectModel InternalClone()
    {
        var clone = (AgentAction)AiObjectFactory.CreateInstance(GetType());
        return clone;
    }

    public virtual void OnStart(IAiContext context) { }
    public virtual void OnGoing(IAiContext context) { OnStart(context);}
    public virtual void OnEnd(IAiContext context) { }

    protected override string GetNameFormat(string currentName)
    {
        return currentName;
    }
    
    public override SingleFileState GetSingleFileState()
    {
        return new AgentActionSingleFileState(this);
    }

    protected override async Task RestoreInternalFromFile(SingleFileState sate)
    {
        
    }

    // public override string GetTypeDescription()
    // {
    //     return "Agent Action";
    // }
    // internal override RestoreState GetState()
    // {
    //     return new AgentActionState(Parameters.ToList(), Name, Description, this);
    // }
    //
    // protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    // {
    //     await base.RestoreInternalAsync(s, restoreDebug);
    //     var state = (AgentActionState)s;
    //     Name = state.Name;
    //     Description = state.Description;
    // }
    //
    // protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    // {
    //     await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_AgentAction);
    // }
}

[Serializable]
public class AgentActionSingleFileState : AiObjectModelSingleFileState
{
    public AgentActionSingleFileState()
    {
    }

    public AgentActionSingleFileState(AiObjectModel o) : base(o)
    {
    }
}

// [Serializable]
// public class AgentActionState: AiObjectState
// {
//     public string Name;
//     public string Description;
//     public List<string> Parameters;
//
//     public AgentActionState(): base()
//     {
//     }
//
//     public AgentActionState(List<Parameter> parameters, string name, string description, AgentAction action): base(action)
//     {
//         Name = name;
//         Description = description;
//         Parameters = RestoreAbleService.NamesToList(parameters);
//
//     }
// }
