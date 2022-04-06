using System;
using System.Collections.Generic;
using UniRxExtension;
using UniRx;
using System.Linq;

public class Decision: UtilityContainer
{
    private IDisposable agentActionSub;
    private ReactiveListNameSafe<AgentAction> agentActions = new ReactiveListNameSafe<AgentAction>();
    public List<Parameter> Parameters;
    public TickMetaData LastSelectedTickMetaData;

    public ReactiveListNameSafe<AgentAction> AgentActions
    {
        get=> agentActions;
        set
        {
            agentActions = value;
            if (agentActions != null)
            {
                agentActionSub?.Dispose();
                UpdateInfo();
                agentActionSub = agentActions.OnValueChanged
                    .Subscribe(_ => UpdateInfo());
            }
        }
    }

    internal override void SetIndex(int value)
    {
        index = value;
        ContextAdress = "D" + index;
    }

    public override string GetNameFormat(string name)
    {
        return name;
    }

    public Decision()
    {
        Parameters = new List<Parameter>(GetParameters());
        agentActionSub?.Dispose();
        UpdateInfo();
        agentActionSub = agentActions.OnValueChanged
            .Subscribe(_ => UpdateInfo());
    }

    public Decision(Decision original): base(original)
    {
        Parameters = new List<Parameter>();
        foreach (var s in original.Parameters)
        {
            var clone = new Parameter(s.Name, s.Value);
            Parameters.Add(clone);
        }
        agentActionSub?.Dispose();

        AgentActions = new ReactiveListNameSafe<AgentAction>();
        foreach(var a in original.AgentActions.Values)
        {
            var clone = a.Clone() as AgentAction;
            AgentActions.Add(clone);
        }
        UpdateInfo();
        agentActionSub = agentActions.OnValueChanged
            .Subscribe(_ => UpdateInfo());
    }

    protected virtual List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    public override void SetContextAddress(string address)
    {
        base.SetContextAddress(address);
        foreach(var a in AgentActions.Values)
        {
            a.SetContextAddress(ContextAddress + "A" + AgentActions.Values.IndexOf(a));
        }
    }

    protected override void UpdateInfo()
    {
        base.UpdateInfo();
        if (AgentActions.Count <= 0)
        {
            Info = new InfoModel("No AgentActions, Object won't be selected", InfoTypes.Warning);
        }
        else if (Considerations.Count <= 0)
        {
            Info = new InfoModel("No Considerations, Object won't be selected", InfoTypes.Warning);
        }
        else
        {
            Info = new InfoModel();
        }
        SetContextAddress(ContextAddress);
    }

    internal override RestoreState GetState()
    {
        return new DecisionState(Name, Description, AgentActions.Values, Considerations.Values, Parameters, this);
    }


    internal override AiObjectModel Clone()
    {
        return new Decision(this);
    }


    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        var state = (DecisionState)s;
        Name = state.Name;
        Description = state.Description;

        AgentActions = new ReactiveListNameSafe<AgentAction>();
        var agentActions = RestoreAbleService.GetAiObjects<AgentAction>(CurrentDirectory + Consts.FolderName_AgentActions, restoreDebug);
        AgentActions.Add(RestoreAbleService.SortByName(state.AgentActions, agentActions));

        Considerations = new ReactiveListNameSafe<Consideration>();
        var considerations = RestoreAbleService.GetAiObjects<Consideration>(CurrentDirectory + Consts.FolderName_Considerations, restoreDebug);
        Considerations.Add(RestoreAbleService.SortByName(state.Considerations, considerations));

        var parameters = RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
        Parameters = RestoreAbleService.SortByName(state.Parameters, parameters);

        if (restoreDebug)
        {
            LastCalculatedUtility = state.LastCalculatedUtility;
        }
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        agentActionSub?.Dispose();
    }

    protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        persister.SaveObject(state, path + "." + Consts.FileExtension_Decision);
        foreach(var aa in AgentActions.Values)
        {
            var subPath = path + "/" + Consts.FolderName_AgentActions;
            aa.SaveToFile(subPath, persister);
        }

        foreach(var c in Considerations.Values)
        {
            var subPath = path + "/" + Consts.FolderName_Considerations;
            c.SaveToFile(subPath, persister);
        }

        foreach(var p in Parameters)
        {
            var subPath = path + "/" + Consts.FolderName_Parameters;
            p.SaveToFile(subPath, persister);
        }
    }

}

[Serializable]
public class DecisionState: RestoreState
{
    public string Name;
    public string Description;
    public float LastCalculatedUtility;

    public List<string> Considerations;
    public List<string> AgentActions;
    public List<string> Parameters;

    public DecisionState() : base()
    {
    }

    public DecisionState(string name, string description, List<AgentAction> agentActions, List<Consideration> considerations, List<Parameter> parameters, Decision o) : base(o)
    {
        Name = name;
        Description = description;
        LastCalculatedUtility = o.LastCalculatedUtility;
        Considerations = RestoreAbleService.NamesToList(considerations);
        AgentActions = RestoreAbleService.NamesToList(agentActions);
        Parameters = RestoreAbleService.NamesToList(parameters);

    }

}
