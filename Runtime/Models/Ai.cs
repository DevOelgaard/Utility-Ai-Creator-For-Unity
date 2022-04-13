using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UniRx;
using UnityEngine;

public class Ai: AiObjectModel
{
    private IDisposable bucketSub;
    private IDisposable playableSub;
    private bool isPLayable;
    public bool IsPLayable
    {
        get { return isPLayable; }
        set
        {
            isPLayable = value;
            onIsPlayableChanged.OnNext(value);
        }
    }
    internal IObservable<bool> OnIsPlayableChanged => onIsPlayableChanged;
    private readonly Subject<bool> onIsPlayableChanged = new Subject<bool> ();
    private ReactiveListNameSafe<Bucket> buckets = new ReactiveListNameSafe<Bucket>();
    public ReactiveListNameSafe<Bucket> Buckets
    {
        get => buckets;
        private set
        {
            buckets = value;
            bucketSub?.Dispose();

            if (buckets == null) return;
            UpdateInfo();
            bucketSub = buckets.OnValueChanged
                .Subscribe(_ => UpdateInfo());
        }
    }
    internal readonly AiContext Context = new AiContext();

    public Ai(): base()
    {
        UpdateInfo();
        bucketSub = buckets.OnValueChanged
            .Subscribe(_ => UpdateInfo());

        playableSub?.Dispose();
        playableSub = OnIsPlayableChanged
            .Subscribe(_ => UpdateInfo());
    }
    protected override AiObjectModel InternalClone()
    {
        var clone = (Ai) Activator.CreateInstance(GetType());
        clone.Buckets = new ReactiveListNameSafe<Bucket>();
        foreach (var c in Buckets.Values.Select(b => b.Clone() as Bucket))
        {
            clone.Buckets.Add(c);
        }
        clone.bucketSub?.Dispose();
        clone.UpdateInfo();
        clone.bucketSub = clone.buckets.OnValueChanged
            .Subscribe(_ => clone.UpdateInfo());

        clone.playableSub?.Dispose();
        clone.playableSub = clone.OnIsPlayableChanged
            .Subscribe(_ => clone.UpdateInfo());
        return clone;
    }

    protected override void UpdateInfo()
    {
        
        base.UpdateInfo();
        if (Buckets == null || Buckets.Count <= 0)
        {
            Info = new InfoModel("No Buckets, Object won't be selected", InfoTypes.Warning);
        }
        else if (!IsPLayable)
        {
            Info = new InfoModel("Not marked playable, Ai won't be selected", InfoTypes.Warning);
        }
        else
        {
            Info = new InfoModel();
        }

        foreach(var b in Buckets.Values)
        {
            b.SetContextAddress("B" + Buckets.Values.IndexOf(b));
        }
    }

    internal override RestoreState GetState()
    {
        return new AiState(Name, Description, Buckets.Values, this);
    }



    private UtilityContainerSelector currentBucketSelector;
    public UtilityContainerSelector CurrentBucketSelector
    {
        get
        {
            if(currentBucketSelector == null)
            {
                currentBucketSelector = BucketSelectors.FirstOrDefault();
            }
            return currentBucketSelector;
        }
        set
        {
            currentBucketSelector = value;
        }
    }
    private List<UtilityContainerSelector> bucketSelectors;
    internal List<UtilityContainerSelector> BucketSelectors
    {
        get
        {
            if (bucketSelectors == null)
            {
                bucketSelectors = new List<UtilityContainerSelector>(AssetDatabaseService.GetInstancesOfType<UtilityContainerSelector>());
            }
            return bucketSelectors;
        }
        set { bucketSelectors = value; }
    }

    private UtilityContainerSelector currentDecisionSelector;
    public UtilityContainerSelector CurrentDecisionSelector
    {
        get
        {
            if(currentDecisionSelector == null)
            {
                currentDecisionSelector = DecisionSelectors.FirstOrDefault();
            }
            return currentDecisionSelector;
        }
        set
        {
            currentDecisionSelector = value;
        }
    }
    private List<UtilityContainerSelector> decisionSelectors;
    internal List<UtilityContainerSelector> DecisionSelectors
    {
        get
        {
            if (decisionSelectors == null)
            {
                decisionSelectors = new List<UtilityContainerSelector>(AssetDatabaseService.GetInstancesOfType<UtilityContainerSelector>());
            }
            return decisionSelectors;
        }
        set { decisionSelectors = value; }
    }

    private IUtilityScorer utilityScorer;
    internal IUtilityScorer UtilityScorer
    {
        get
        {
            if (utilityScorer == null)
            {
                utilityScorer = AssetDatabaseService.GetInstancesOfType<IUtilityScorer>()
                    .FirstOrDefault(e => e.GetName() == Consts.Default_UtilityScorer);
            }
            return utilityScorer;
        }
        set
        {
            utilityScorer = value;
            Context.UtilityScorer = UtilityScorer;
        }
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        bucketSub?.Dispose();
        playableSub?.Dispose();
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var state = (AiState)s;
        Name = state.Name;
        Description = state.Description;
        IsPLayable = state.IsPLayable;

        var task = Task.Factory.StartNew(() =>
        {
            Buckets = new ReactiveListNameSafe<Bucket>();
            var bucketsLocal = RestoreAbleService.GetAiObjects<Bucket>(CurrentDirectory + Consts.FolderName_Buckets, restoreDebug);
            Buckets.Add(RestoreAbleService.SortByName(state.Buckets, bucketsLocal));

            BucketSelectors = RestoreAbleService.GetUCS(CurrentDirectory + Consts.FolderName_BucketSelectors, restoreDebug);
            CurrentBucketSelector = BucketSelectors
                .FirstOrDefault(d => d.GetName() == state.CurrentBucketSelectorName);

            if (CurrentBucketSelector == null)
            {
                CurrentBucketSelector = BucketSelectors.FirstOrDefault();
            }

            DecisionSelectors = RestoreAbleService.GetUCS(CurrentDirectory + Consts.FolderName_DecisionSelectors, restoreDebug);
            CurrentDecisionSelector = DecisionSelectors
                .FirstOrDefault(d => d.GetName() == state.CurrentDecisionSelectorName);

            if (CurrentDecisionSelector == null)
            {
                CurrentDecisionSelector = DecisionSelectors.FirstOrDefault();
            }

            var utilityScorers = AssetDatabaseService.GetInstancesOfType<IUtilityScorer>();
            UtilityScorer = utilityScorers
                .FirstOrDefault(u => u.GetName() == state.USName);

            if (UtilityScorer == null)
            {
                UtilityScorer = utilityScorers.FirstOrDefault();
            }
        });
        await task;
    }

    protected override void InternalSaveToFile(string path, IPersister destructivePersister, RestoreState state)
    {
        destructivePersister.SaveObject(state, path + "." + Consts.FileExtension_UAI);
        foreach(var b in Buckets.Values)
        {
            var subPath = path + "/" + Consts.FolderName_Buckets;
            b.SaveToFile(subPath,destructivePersister);
        }

        foreach(var bs in BucketSelectors)
        {
            var subPath = path + "/" + Consts.FolderName_BucketSelectors;
            bs.SaveToFile(subPath, destructivePersister);
        }

        foreach (var ds in DecisionSelectors)
        {
            var subPath = path + "/" + Consts.FolderName_DecisionSelectors;
            ds.SaveToFile(subPath, destructivePersister);
        }
    }
}

public class AiState: RestoreState
{
    public string Name;
    public string Description;
    public bool IsPLayable;
    public string CurrentBucketSelectorName;
    public string CurrentDecisionSelectorName;
    public string USName;

    public List<string> Buckets;

    public AiState() : base()
    {
    }

    public AiState(string name, string description, List<Bucket> buckets, Ai model): base(model)
    {
        Name = name;
        Description = description;
        IsPLayable = model.IsPLayable;
        
        Buckets = RestoreAbleService.NamesToList(buckets);

        CurrentBucketSelectorName = model.CurrentBucketSelector.GetName();
        CurrentDecisionSelectorName = model.CurrentDecisionSelector.GetName();

        USName = model.UtilityScorer.GetName();
    }
}