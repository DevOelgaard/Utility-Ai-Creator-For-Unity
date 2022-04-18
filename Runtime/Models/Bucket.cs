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

    protected override AiObjectModel InternalClone()
    {
        var clone = (Bucket)Activator.CreateInstance(GetType());
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

    protected override float CalculateUtility(AiContext context)
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
    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var task = Task.Factory.StartNew(() =>
        {
            var state = (BucketState)s;
            Name = state.Name;
            Description = state.Description;

            Decisions = new ReactiveListNameSafe<Decision>();
            var decisionsLocal = RestoreAbleService.GetAiObjects<Decision>(CurrentDirectory + Consts.FolderName_Decisions, restoreDebug);
            Decisions.Add(RestoreAbleService.SortByName(state.Decisions, decisionsLocal));
        
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
        });
        await task;
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        decisionSub?.Dispose();
    }

    protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        persister.SaveObject(state, path + "." + Consts.FileExtension_Bucket);
        RestoreAbleService.SaveRestoreAblesToFile(Decisions.Values,path + "/" + Consts.FolderName_Decisions, persister);
        RestoreAbleService.SaveRestoreAblesToFile(Considerations.Values,path + "/" + Consts.FolderName_Considerations, persister);
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