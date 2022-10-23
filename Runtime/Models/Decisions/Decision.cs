using System;
using System.Collections.Generic;
using UniRxExtension;
using UniRx;
using System.Linq;
using System.Threading.Tasks;

public class Decision: UtilityContainer
{
    private IDisposable agentActionSub;
    private ReactiveListNameSafe<AgentAction> agentActions = new ReactiveListNameSafe<AgentAction>();
    public TickMetaData LastSelectedTickMetaData;
    private float weight = 1f;
    // private float baseWeight = 1f;

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
        Index = value;
    }

    protected override string GetNameFormat(string name)
    {
        return name;
    }

    public Decision()
    {
        agentActionSub = agentActions.OnValueChanged
            .Subscribe(_ => UpdateInfo());
        BaseAiObjectType = typeof(Decision);
        AddParameter("Base Weight", 1f);
    }

    public override float GetWeight()
    {
        return ParameterContainer.GetParamFloat("Base Weight").Value;
    }

    protected override AiObjectModel InternalClone()
    {
        var clone = (Decision)AiObjectFactory.CreateInstance(GetType());
        
        clone.agentActionSub?.Dispose();
        clone.AgentActions = new ReactiveListNameSafe<AgentAction>();
        foreach (var a in AgentActions.Values)
        {
            var aClone = a.Clone() as AgentAction;
            clone.AgentActions.Add(aClone);
        }
        clone.UpdateInfo();
        clone.agentActionSub = clone.agentActions.OnValueChanged
            .Subscribe(_ => clone.UpdateInfo());

        return clone;
    }

    public override void SetParent(AiObjectModel parent, int indexInParent)
    {
        base.SetParent(parent,indexInParent);
        foreach(var a in AgentActions.Values)
        {
            a.SetParent(this,AgentActions.Values.IndexOf(a));
        }

        foreach (var consideration in Considerations.Values)
        {
            consideration.SetParent(this,Considerations.Values.IndexOf(consideration));
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

        foreach (var agentAction in AgentActions.Values)
        {
            agentAction.SetParent(this,AgentActions.Values.IndexOf(agentAction));
        }
    }

    protected override float CalculateUtility(IAiContext context)
    {
        var modifier = float.NaN;
        foreach(var cons in Considerations.Values.Where(c => c.IsModifier))
        {
            modifier = cons.CalculateScore(context);
        }
        var parent = ContextAddress.Parent as Bucket;

        if (!float.IsNaN(modifier))
        {
            weight = modifier;
        }
        else
        {
            weight = GetWeight();
        }
        return base.CalculateUtility(context) * parent.Utility;
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        agentActionSub?.Dispose();
    }

    protected override async Task RestoreInternalFromFile(SingleFileState state)
    {
        var s = (DecisionSingleFileState) state;

        AgentActions = new ReactiveListNameSafe<AgentAction>();
        foreach (var agentActionState in s.agentActionStates)
        {
            AgentActions.Add(await Restore<AgentAction>(agentActionState));
        }
        
        Considerations = new ReactiveListNameSafe<Consideration>();
        foreach (var considerationState in s.considerationsStates)
        {
            Considerations.Add(await Restore<Consideration>(considerationState));
        }
        Utility = s.lastCalculatedUtility;
    }
    // protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    // {
    //     await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_Decision);
    //     await RestoreAbleService.SaveRestoreAblesToFile(AgentActions.Values,path + "/" + Consts.FolderName_AgentActions, persister);
    //     await RestoreAbleService.SaveRestoreAblesToFile(Considerations.Values,path + "/" + Consts.FolderName_Considerations, persister);
    //     // await RestoreAbleService.SaveRestoreAblesToFile(Parameters,path + "/" + Consts.FolderName_Parameters, persister);
    // }
    public override SingleFileState GetSingleFileState()
    {
        return new DecisionSingleFileState(this);
    }
}

// [Serializable]
// public class DecisionState: AiObjectState
// {
//     public string Name;
//     public string Description;
//     public float LastCalculatedUtility;
//
//     public List<string> Considerations;
//     public List<string> AgentActions;
//     public List<string> Parameters;
//
//     public DecisionState() : base()
//     {
//     }
//
//     public DecisionState(string name, string description, List<AgentAction> agentActions, List<Consideration> considerations, List<Parameter> parameters, Decision o) : base(o)
//     {
//         Name = name;
//         Description = description;
//         LastCalculatedUtility = o.LastCalculatedUtility;
//         Considerations = RestoreAbleService.NamesToList(considerations);
//         AgentActions = RestoreAbleService.NamesToList(agentActions);
//         Parameters = RestoreAbleService.NamesToList(parameters);
//
//     }
// }

[Serializable]
public class DecisionSingleFileState : AiObjectModelSingleFileState
{
    public float lastCalculatedUtility;
    public List<ConsiderationSingleFileState> considerationsStates;
    public List<AgentActionSingleFileState> agentActionStates;

    public DecisionSingleFileState()
    {
    }

    public DecisionSingleFileState(Decision o) : base(o)
    {
        considerationsStates = new List<ConsiderationSingleFileState>();
        foreach (var consideration in o.Considerations.Values)
        {
            considerationsStates.Add(consideration.GetSingleFileState() as ConsiderationSingleFileState);
        }
        
        agentActionStates = new List<AgentActionSingleFileState>();
        foreach (var agentAction in o.AgentActions.Values)
        {
            agentActionStates.Add(agentAction.GetSingleFileState() as AgentActionSingleFileState);
        }
    }
}
