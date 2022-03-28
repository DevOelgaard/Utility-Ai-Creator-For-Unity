using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UniRx;

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

    protected virtual List<Parameter> GetParameters()
    {
        return new List<Parameter>();
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
    }

    internal override string GetContextAddress(AiContext context)
    {
        return context.CurrentEvaluatedBucket.Name + Name;
    }

    internal override RestoreState GetState()
    {
        return new DecisionState(Name, Description, AgentActions.Values, Considerations.Values, Parameters, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<Decision>(state);
        return clone;
    }


    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        var state = (DecisionState)s;
        Name = state.Name;
        Description = state.Description;

        AgentActions = new ReactiveListNameSafe<AgentAction>();
        var agentActions = new List<AgentAction>();
        foreach (var a in state.AgentActions)
        {
            var action = AgentAction.Restore<AgentAction>(a, restoreDebug);
            agentActions.Add(action);
        }
        AgentActions.Add(agentActions);

        Considerations = new ReactiveListNameSafe<Consideration>();
        var considerations = new List<Consideration>();
        foreach (var c in state.Considerations)
        {
            var consideration = Consideration.Restore<Consideration>(c, restoreDebug);
            considerations.Add(consideration);
        }
        Considerations.Add(considerations);

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

}

[Serializable]
public class DecisionState: RestoreState
{
    public string Name;
    public string Description;
    public List<AgentActionState> AgentActions;
    public List<ConsiderationState> Considerations;
    public List<ParameterState> Parameters;
    public float LastCalculatedUtility;
    public DecisionState() : base()
    {
    }

    public DecisionState(string name, string description, List<AgentAction> agentActions, List<Consideration> considerations, List<Parameter> parameters, Decision o) : base(o)
    {
        Name = name;
        Description = description;

        AgentActions = new List<AgentActionState>();
        foreach (AgentAction action in agentActions)
        {
            var a = action.GetState() as AgentActionState;
            AgentActions.Add(a);
        }

        Considerations = new List<ConsiderationState>();
        foreach (Consideration consideration in considerations)
        {
            var c = consideration.GetState() as ConsiderationState;
            Considerations.Add(c);
        }

        Parameters = new List<ParameterState>();
        foreach (var parameter in parameters)
        {
            Parameters.Add(parameter.GetState() as ParameterState);
        }
        LastCalculatedUtility = o.LastCalculatedUtility;
    }

}
