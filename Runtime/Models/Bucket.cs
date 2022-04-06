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

    public Bucket(Bucket original): base(original)
    {
        Decisions = new ReactiveListNameSafe<Decision>();
        foreach(var d in original.Decisions.Values)
        {
            var clone = new Decision(d);
            Decisions.Add(d);
        }
        decisionSub?.Dispose();
        UpdateInfo();
        decisionSub = decisions.OnValueChanged
            .Subscribe(_ => UpdateInfo());

        Weight = new Parameter(original.Weight.Name, original.Weight.Value);
    }

    internal override AiObjectModel Clone()
    {
        return new Bucket(this);
    }

    protected override float CalculateUtility(AiContext context)
    {
        return context.UtilityScorer.CalculateUtility(Considerations.Values, context) * Convert.ToSingle(Weight.Value);
    }

    public override void SetContextAddress(string address)
    {
        base.SetContextAddress(address);
        foreach (var d in Decisions.Values)
        {
            d.SetContextAddress(ContextAddress+ "D" + Decisions.Values.IndexOf(d));
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
        SetContextAddress(ContextAddress);
    }


    internal override RestoreState GetState()
    {
        return new BucketState(Name, Description, Decisions.Values, Considerations.Values, Weight, this);
    }
    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        var state = (BucketState)s;
        Name = state.Name;
        Description = state.Description;

        Decisions = new ReactiveListNameSafe<Decision>();
        var decisions = RestoreAbleService.GetAiObjects<Decision>(CurrentDirectory + Consts.FolderName_Decisions, restoreDebug);
        Decisions.Add(RestoreAbleService.SortByName(state.Decisions, decisions));//var decisions = new List<Decision>();
        
        Considerations = new ReactiveListNameSafe<Consideration>();
        var considerations = RestoreAbleService.GetAiObjects<Consideration>(CurrentDirectory + Consts.FolderName_Considerations, restoreDebug);
        Considerations.Add(RestoreAbleService.SortByName(state.Considerations, considerations));

        var weightState = PersistenceAPI.Instance.LoadObjectsPath<ParameterState>(CurrentDirectory + Consts.FolderName_Weight).FirstOrDefault();
        if(weightState.LoadedObject == null)
        {
            Weight = new Parameter(weightState.ErrorMessage, weightState.Exception.ToString());
        }
        else
        {
            Weight = Restore<Parameter>(weightState.LoadedObject);
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

    protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        persister.SaveObject(state, path + "." + Consts.FileExtension_Bucket);
        foreach(var d in Decisions.Values)
        {
            var subPath = path + "/" + Consts.FolderName_Decisions;
            d.SaveToFile(subPath, persister);
        }

        foreach (var c in Considerations.Values)
        {
            var subPath = path + "/" + Consts.FolderName_Considerations;
            c.SaveToFile(subPath, persister);
        }

        var wSubPath = path + "/" + Consts.FolderName_Weight;
        Weight.SaveToFile(wSubPath, persister);
    }
}

[Serializable]
public class BucketState: RestoreState
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