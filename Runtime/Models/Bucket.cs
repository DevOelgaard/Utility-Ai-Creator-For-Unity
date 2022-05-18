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
    public Parameter Weight;

    public Bucket(): base()
    {
        decisionSub = decisions.OnValueChanged
            .Subscribe(_ => UpdateInfo());
        Weight = new Parameter("Weight", 1f);
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

        clone.Weight = Weight.Clone();
        return clone;
    }

    protected override float CalculateUtility(IAiContext context)
    {
        var modifier = float.NaN;
        foreach (var cons in Considerations.Values.Where(c => c.IsModifier))
        {
            modifier = cons.CalculateScore(context);
        }
        if (float.IsNaN(modifier))
        {
            return base.CalculateUtility(context) * Convert.ToSingle(Weight.Value);
        }
        else
        {
            return base.CalculateUtility(context) * modifier;
        }
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


    internal override RestoreState GetState()
    {
        return new BucketState(Name, Description, Decisions.Values, Considerations.Values, Weight, this);
    }
    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var state = (BucketState)s;
        Name = state.Name;
        Description = state.Description;

        Decisions = new ReactiveListNameSafe<Decision>();
        var decisionsLocal = await RestoreAbleService
            .GetAiObjectsSortedByIndex<Decision>(CurrentDirectory + Consts.FolderName_Decisions, restoreDebug);
        
        Decisions.Add(RestoreAbleService.OrderByNames(state.Decisions, decisionsLocal));
        
        Considerations = new ReactiveListNameSafe<Consideration>();
        var considerations = await RestoreAbleService
            .GetAiObjectsSortedByIndex<Consideration>(CurrentDirectory + Consts.FolderName_Considerations, restoreDebug);
        
        Considerations.Add(RestoreAbleService.OrderByNames(state.Considerations, considerations));

        var wStates = await PersistenceAPI.Instance
            .LoadObjectsAsync<ParameterState>(CurrentDirectory + Consts.FolderName_Weight);
        var weightState = wStates.FirstOrDefault();
        if (weightState == null)
        {
            throw new NullReferenceException("WeightState is null. State: " + state.Name);
        }
        if(weightState.LoadedObject == null)
        {
            Weight = new Parameter(weightState.ErrorMessage, weightState.Exception.ToString());
        }
        else
        {
            Weight = await Restore<Parameter>(weightState.LoadedObject);
        }


        if (restoreDebug)
        {
            LastCalculatedUtility = state.LastCalculatedUtility;
        }
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        decisionSub?.Dispose();
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_Bucket);
        await RestoreAbleService.SaveRestoreAblesToFile(Decisions.Values,path + "/" + Consts.FolderName_Decisions, persister);
        await RestoreAbleService.SaveRestoreAblesToFile(Considerations.Values,path + "/" + Consts.FolderName_Considerations, persister);
        var wSubPath = path + "/" + Consts.FolderName_Weight;
        await Weight.SaveToFile(wSubPath, persister);
    }
}

[Serializable]
public class BucketState: AiObjectState  
{
    public string Name;
    public string Description;
    public float LastCalculatedUtility;

    public List<string> Decisions;
    public List<string> Considerations;

    public BucketState(): base()
    {
    }

    public BucketState(string name, string description, List<Decision> decisions, List<Consideration> considerations, Parameter weight,  Bucket o) : base(o)
    {
        this.Name = name;
        this.Description = description;
        
        Decisions = RestoreAbleService.NamesToList(decisions);
        Considerations = RestoreAbleService.NamesToList(considerations);
        LastCalculatedUtility = o.LastCalculatedUtility;
    }
}