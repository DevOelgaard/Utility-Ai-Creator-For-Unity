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
    private Subject<bool> onIsPlayableChanged = new Subject<bool> ();
    private ReactiveListNameSafe<Bucket> buckets = new ReactiveListNameSafe<Bucket>();
    public ReactiveListNameSafe<Bucket> Buckets
    {
        get => buckets;
        set
        {
            buckets = value;
            bucketSub?.Dispose();

            if (buckets != null)
            {
                UpdateInfo();
                bucketSub = buckets.OnValueChanged
                    .Subscribe(_ => UpdateInfo());
            }
        }
    }
    internal AiContext Context = new AiContext();

    public Ai(): base()
    {
        bucketSub?.Dispose();
        UpdateInfo();
        bucketSub = buckets.OnValueChanged
            .Subscribe(_ => UpdateInfo());

        playableSub?.Dispose();
        playableSub = OnIsPlayableChanged
            .Subscribe(_ => UpdateInfo());
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
    }

    internal override RestoreState GetState()
    {
        return new UAIModelState(Name, Description, Buckets.Values, this);
    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<Ai>(state);
        return clone;
    }

    protected override void RestoreInternal(RestoreState s, bool restoreLog = false)
    {
        var state = (UAIModelState)s;
        Name = state.Name;
        Description = state.Description;
        IsPLayable = state.IsPLayable;

        Buckets = new ReactiveListNameSafe<Bucket>();
        var buckets = new List<Bucket>();
        foreach(var bS in state.Buckets)
        {
            var b = Bucket.Restore<Bucket>(bS, restoreLog);
            buckets.Add(b);
        }
        Buckets.Add(buckets);

        BucketSelectors = new List<UtilityContainerSelector>();
        foreach(var bS in state.BucketSelectors)
        {
            var ucs = Restore<UtilityContainerSelector>(bS, restoreLog);
            BucketSelectors.Add(ucs);
        }

        CurrentBucketSelector = BucketSelectors
            .FirstOrDefault(d => d.GetName() == state.CurrentBucketSelectorName);

        if (CurrentBucketSelector == null)
        {
            CurrentBucketSelector = BucketSelectors.FirstOrDefault();
        }

        DecisionSelectors = new List<UtilityContainerSelector>();
        foreach(var ds in state.DecisionSelectors)
        {
            var ucs = Restore<UtilityContainerSelector>(ds, restoreLog);
            DecisionSelectors.Add(ucs);
        }
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
            //Debug.LogWarning("No Utility Scorer found");
            UtilityScorer = utilityScorers.FirstOrDefault();
        }
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
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
}

public class UAIModelState: RestoreState
{
    public string Name;
    public string Description;
    public bool IsPLayable;
    public List<BucketState> Buckets = new List<BucketState>();
    public string CurrentBucketSelectorName;
    public string CurrentDecisionSelectorName;
    public List<UCSState> BucketSelectors;
    public List<UCSState> DecisionSelectors;
    public string USName;

    public UAIModelState() : base()
    {
    }

    public UAIModelState(string name, string description, List<Bucket> buckets, Ai model): base(model)
    {
        Name = name;
        Description = description;
        IsPLayable = model.IsPLayable;
        Buckets = new List<BucketState>();
        foreach(var b in buckets)
        {
            var bS = b.GetState() as BucketState;
            Buckets.Add(bS);
        }

        CurrentBucketSelectorName = model.CurrentBucketSelector.GetName();
        CurrentDecisionSelectorName = model.CurrentDecisionSelector.GetName();

        BucketSelectors = new List<UCSState>();
        foreach(var ucs in model.BucketSelectors)
        {
            var bS = ucs.GetState() as UCSState;
            BucketSelectors.Add(bS);
        }

        DecisionSelectors = new List<UCSState>();
        foreach(var ucs in model.DecisionSelectors)
        {
            var dS = ucs.GetState() as UCSState;
            DecisionSelectors.Add(dS);
        }

        USName = model.UtilityScorer.GetName();
    }
}