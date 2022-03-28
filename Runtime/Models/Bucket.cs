using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UniRx;

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
    public Parameter Weight;

    public Bucket(): base()
    {
        decisionSub?.Dispose();
        UpdateInfo();
        decisionSub = decisions.OnValueChanged
            .Subscribe(_ => UpdateInfo());
        Weight = new Parameter("Weight", 1f);
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<Bucket>(state);
        return clone;
    }

    protected override float CalculateUtility(AiContext context)
    {
        return context.UtilityScorer.CalculateUtility(Considerations.Values, context) * Convert.ToSingle(Weight.Value);
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
    }

    internal override RestoreState GetState()
    {
        return new BucketState(Name, Description, Decisions.Values, Considerations.Values, Weight, this);
    }
    protected override void RestoreInternal(RestoreState state, bool restoreDebug = false)
    {
        var stateCast = (BucketState)state;
        Name = stateCast.Name;
        Description = stateCast.Description;

        Decisions = new ReactiveListNameSafe<Decision>();
        var decisions = new List<Decision>();
        foreach (var d in stateCast.Decisions)
        {
            var decision = Restore<Decision>(d, restoreDebug);
            decisions.Add(decision);
        }
        Decisions.Add(decisions);

        Considerations = new ReactiveListNameSafe<Consideration>();
        var considerations = new List<Consideration>();
        foreach (var c in stateCast.Considerations)
        {
            var consideration = Restore<Consideration>(c, restoreDebug);
            considerations.Add(consideration);
        }
        Considerations.Add(considerations);

        Weight = Restore<Parameter>(stateCast.Weight);

        if (restoreDebug)
        {
            LastCalculatedUtility = stateCast.LastCalculatedUtility;
        }
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        decisionSub?.Dispose();
    }

    //internal override void SaveToFile(string path, IPersister persister)
    //{
    //    var state = GetState();
    //    persister.SaveObject(state, path);
    //}
}

[Serializable]
public class BucketState: RestoreState
{
    public string Name;
    public string Description;
    public List<DecisionState> Decisions;
    public List<ConsiderationState> Considerations;
    public ParameterState Weight;
    public float LastCalculatedUtility;

    public BucketState(): base()
    {
    }

    public BucketState(string name, string description, List<Decision> decisions, List<Consideration> considerations, Parameter weight,  Bucket o) : base(o)
    {
        this.Name = name;
        this.Description = description;
        this.Decisions = new List<DecisionState>();
        foreach (var d in decisions)
        {
            var state = d.GetState() as DecisionState;
            Decisions.Add(state);
        }

        this.Considerations = new List<ConsiderationState>();
        foreach (var c in considerations)
        {
            var state = c.GetState() as ConsiderationState;
            Considerations.Add(state);
        }

        this.Weight = weight.GetState() as ParameterState;
        LastCalculatedUtility = o.LastCalculatedUtility;
    }
}