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
        Parameters = new List<Parameter>(GetParameters());
    }

    //protected AgentAction(AgentAction original): base(original)
    //{
    //    Parameters = new List<Parameter>();
    //    foreach (var s in original.Parameters)
    //    {
    //        var clone = new Parameter(s.Name, s.Value);
    //        Parameters.Add(clone);
    //    }
    //}

    protected virtual List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    public override string GetTypeDescription()
    {
        return "Agent Action";
    }
    protected override AiObjectModel InternalClone()
    {
        var clone = (AgentAction)Activator.CreateInstance(GetType());
        clone.Parameters = new List<Parameter>();
        foreach (var s in this.Parameters)
        {
            var c = s.Clone();
            clone.Parameters.Add(c);
        }
        return clone;
    }

    public virtual void OnStart(AiContext context) { }
    public virtual void OnGoing(AiContext context) { }
    public virtual void OnEnd(AiContext context) { }

    protected override string GetNameFormat(string currentName)
    {
        return currentName;
    }

    internal override RestoreState GetState()
    {
        return new AgentActionState(Parameters, Name, Description, this);
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var state = (AgentActionState)s;
        Name = state.Name;
        Description = state.Description;

        var parameters = await RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
        Parameters = RestoreAbleService.SortByName(state.Parameters, parameters);
    }

    protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        persister.SaveObject(state, path + "." + Consts.FileExtension_AgentAction);
        RestoreAbleService.SaveRestoreAblesToFile(Parameters,path + "/" + Consts.FolderName_Parameters, persister);
    }
}

[Serializable]
public class AgentActionState: RestoreState
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
