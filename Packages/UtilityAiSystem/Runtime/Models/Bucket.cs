using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UniRx;
using UnityEditor.Search;
using UnityEngine;

public class Bucket : UtilityContainer 
{
    private IDisposable decisionSub;
    private ReactiveListNameSafe<Decision> decisions = new ReactiveListNameSafe<Decision>();
    public ReactiveListNameSafe<Decision> Decisions
    {
        get => decisions;
        set
        {
            decisions = value;
            if (decisions != null)
            {
                decisionSub?.Dispose();
                UpdateInfo();
                decisionSub = decisions.OnValueChanged
                    .Subscribe(_ => UpdateInfo());
            }
        }
    }
    public ParamFloat Weight;

    public Bucket(): base()
    {
        decisionSub = decisions.OnValueChanged
            .Subscribe(_ => UpdateInfo());
        Weight = new ParamFloat("Weight", 1f);
        BaseAiObjectType = typeof(Bucket);
    }


    protected override AiObjectModel InternalClone()
    {
        var clone = (Bucket)AiObjectFactory.CreateInstance(GetType());
        clone.Decisions = new ReactiveListNameSafe<Decision>();
        foreach (var d in Decisions.Values)
        {
            var dClone = d.Clone() as Decision;
            clone.Decisions.Add(dClone);
        }
        clone.decisionSub?.Dispose();
        clone.UpdateInfo();
        clone.decisionSub = clone.decisions.OnValueChanged
            .Subscribe(_ => clone.UpdateInfo());

        clone.Weight = Weight.Clone() as ParamFloat;
        return clone;
    }

    private bool firstCalculation = true;
    internal float baseWeight { get; private set; }

    protected override float CalculateUtility(IAiContext context)
    {
        if (firstCalculation)
        {
            baseWeight = Convert.ToSingle(Weight.Value);
            firstCalculation = false;
        }
        var modifier = float.NaN;
        foreach (var cons in Considerations.Values.Where(c => c.IsModifier))
        {
            modifier = cons.CalculateScore(context);
        }
        if (float.IsNaN(modifier))
        {
            Weight.Value = baseWeight;
        }
        else
        {
            Weight.Value = modifier;
        }
        return base.CalculateUtility(context) * Convert.ToSingle(Weight.Value);

    }

    public override void SetParent(AiObjectModel parent, int indexInParent)
    {
        base.SetParent(parent,indexInParent);
        foreach (var d in Decisions.Values)
        {
            d.SetParent(this,Decisions.Values.IndexOf(d));
        }
        
        foreach (var consideration in Considerations.Values)
        {
            consideration.SetParent(this,Considerations.Values.IndexOf(consideration));
        }
    }

    protected override void UpdateInfo()
    {
        base.UpdateInfo();
        if (Decisions.Count <= 0)
        {
            Info = new InfoModel("No Decisions, Object won't be selected",InfoTypes.Warning);
        } else if (Considerations.Count <= 0)
        {
            Info = new InfoModel("No Considerations, Object won't be selected", InfoTypes.Warning);
        } else
        {
            Info = new InfoModel();
        }

        foreach (var decision in Decisions.Values)
        {
            decision.SetParent(this,Decisions.Values.IndexOf(decision));
        }
    }

    //
    // internal override RestoreState GetState()
    // {
    //     return new BucketState(Name, Description, Decisions.Values, Considerations.Values, Weight, this);
    // }
    // protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    // {
    //     var state = (BucketState)s;
    //     Name = state.Name;
    //     Description = state.Description;
    //
    //     Decisions = new ReactiveListNameSafe<Decision>();
    //     var decisionsLocal = await RestoreAbleService
    //         .GetAiObjectsSortedByIndex<Decision>(CurrentDirectory + Consts.FolderName_Decisions, restoreDebug);
    //     
    //     Decisions.Add(RestoreAbleService.OrderByNames(state.Decisions, decisionsLocal));
    //     
    //     Considerations = new ReactiveListNameSafe<Consideration>();
    //     var considerations = await RestoreAbleService
    //         .GetAiObjectsSortedByIndex<Consideration>(CurrentDirectory + Consts.FolderName_Considerations, restoreDebug);
    //     
    //     Considerations.Add(RestoreAbleService.OrderByNames(state.Considerations, considerations));
    //
    //     var wStates = await PersistenceAPI.Instance
    //         .LoadObjectsAsync<ParameterState>(CurrentDirectory + Consts.FolderName_Weight);
    //     var weightState = wStates.FirstOrDefault();
    //     if (weightState == null)
    //     {
    //         throw new NullReferenceException("WeightState is null. State: " + state.Name);
    //     }
    //     if(weightState.LoadedObject == null)
    //     {
    //         Weight = new Parameter(weightState.ErrorMessage, weightState.Exception.ToString());
    //     }
    //     else
    //     {
    //         Weight = await Restore<Parameter>(weightState.LoadedObject);
    //     }
    //
    //
    //     if (restoreDebug)
    //     {
    //         LastCalculatedUtility = state.LastCalculatedUtility;
    //     }
    // }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        decisionSub?.Dispose();
    }

    protected override async Task RestoreInternalFromFile(SingleFileState state)
    {
        var s = (BucketSingleFileState)state;

        Decisions = new ReactiveListNameSafe<Decision>();
        foreach (var decisionState in s.decisions)
        {
            Decisions.Add(await Restore<Decision>(decisionState));
        }
        
        Considerations = new ReactiveListNameSafe<Consideration>();
        foreach (var considerationState in s.considerations)
        {
            Considerations.Add(await Restore<Consideration>(considerationState));
        }
        Weight = await RestoreAble.Restore<ParamFloat>(s.weight);
        LastCalculatedUtility = s.lastCalculatedUtility;
    }

    public override SingleFileState GetSingleFileState()
    {
        return new BucketSingleFileState(this);
    }
}

[Serializable]
public class BucketSingleFileState: AiObjectModelSingleFileState  
{
    public float lastCalculatedUtility;
    public List<DecisionSingleFileState> decisions;
    public List<ConsiderationSingleFileState> considerations;
    public ParamBaseState<float> weight;

    public BucketSingleFileState(): base()
    {
    }

    public BucketSingleFileState(Bucket o) : base(o)
    {
        decisions = new List<DecisionSingleFileState>();
        foreach (var oDecision in o.Decisions.Values)
        {
            decisions.Add(oDecision.GetSingleFileState() as DecisionSingleFileState);
        }
        
        considerations = new List<ConsiderationSingleFileState>();
        foreach (var oDecision in o.Considerations.Values)
        {
            considerations.Add(oDecision.GetSingleFileState() as ConsiderationSingleFileState);
        }
        lastCalculatedUtility = o.LastCalculatedUtility;
        weight = o.Weight.GetState() as ParamBaseState<float>;
    }
}