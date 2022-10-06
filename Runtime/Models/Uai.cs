using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;
using UniRx;
using UnityEngine;

public class Uai: AiObjectModel
{
    private IDisposable bucketSub;
    private IDisposable playableSub;
    private bool isPLayAble;
    public bool IsPLayAble
    {
        get => isPLayAble;
        set
        {
            isPLayAble = value;
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

    private IAiContext uaiContext;

    internal IAiContext UaiContext
    {
        get { return uaiContext ??= new AiContext(); }
        private set => uaiContext = value;
    }

    public Uai(): base()
    {
        bucketSub = buckets.OnValueChanged
            .Subscribe(_ => UpdateInfo());

        playableSub?.Dispose();
        playableSub = OnIsPlayableChanged
            .Subscribe(_ => UpdateInfo());
        BaseAiObjectType = typeof(Uai);
    }

    internal void SetContext(IAiContext context)
    {
        if (UaiContext != null) return;
        UaiContext = context;
    }

    protected override AiObjectModel InternalClone()
    {
        var clone = (Uai) AiObjectFactory.CreateInstance(typeof(Uai));

        clone.CurrentBucketSelector = CurrentBucketSelector.Clone();
        clone.CurrentDecisionSelector = CurrentDecisionSelector.Clone();

        clone.BucketSelectors =
            clone.BucketSelectors.Where(bs => bs.GetName() != CurrentBucketSelector.GetName())
                .ToList();
        clone.BucketSelectors.Add(clone.CurrentBucketSelector);

        clone.DecisionSelectors = 
            clone.DecisionSelectors.Where(ds => ds.GetName() != CurrentDecisionSelector.GetName())
            .ToList();
        clone.DecisionSelectors.Add(clone.CurrentDecisionSelector);
        
        clone.UtilityScorer = AssetService.GetInstanceOfType<IUtilityScorer>(this.UtilityScorer.GetType().ToString());
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
        if(Buckets == null || Buckets.Count == 0)
        {
            Info = new InfoModel("No Buckets, Object won't be selected", InfoTypes.Warning);
        }
        else if (!IsPLayAble)
        {
            Info = new InfoModel("Not marked playable, Ai won't be selected", InfoTypes.Warning);
        }
        else
        {
            Info = new InfoModel();
        }
    }

    public override void SetParent(AiObjectModel parent, int indexInParent)
    {
        base.SetParent(parent,indexInParent);
        foreach (var bucket in Buckets.Values)
        {
            bucket.SetParent(this,Buckets.Values.IndexOf(bucket));
        }
    }

    // internal override RestoreState GetState()
    // {
    //     return new UaiState(Name, Description, Buckets.Values, this);
    // }

    private UtilityContainerSelector currentBucketSelector;
    public UtilityContainerSelector CurrentBucketSelector
    {
        get => currentBucketSelector ??= BucketSelectors.FirstOrDefault();
        set => currentBucketSelector = value;
    }
    private List<UtilityContainerSelector> bucketSelectors;
    internal List<UtilityContainerSelector> BucketSelectors
    {
        get =>
            bucketSelectors ??= new List<UtilityContainerSelector>(AssetService
                .GetInstancesOfType<UtilityContainerSelector>());
        set => bucketSelectors = value;
    }

    private UtilityContainerSelector currentDecisionSelector;
    public UtilityContainerSelector CurrentDecisionSelector
    {
        get => currentDecisionSelector ??= DecisionSelectors.FirstOrDefault();
        set => currentDecisionSelector = value;
    }
    private List<UtilityContainerSelector> decisionSelectors;
    internal List<UtilityContainerSelector> DecisionSelectors
    {
        get =>
            decisionSelectors ??= new List<UtilityContainerSelector>(AssetService
                .GetInstancesOfType<UtilityContainerSelector>());
        set => decisionSelectors = value;
    }

    private IUtilityScorer utilityScorer;
    internal IUtilityScorer UtilityScorer
    {
        get
        {
            return utilityScorer ??= AssetService.GetInstancesOfType<IUtilityScorer>()
                .FirstOrDefault(e => e.GetName() == Consts.Default_UtilityScorer);
        }
        set
        {
            utilityScorer = value;
            UaiContext.UtilityScorer = UtilityScorer;
        }
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        bucketSub?.Dispose();
        playableSub?.Dispose();
    }

    protected override async Task RestoreInternalFromFile(SingleFileState state)
    {
        {
            var s = (UaiSingleFileState)state;
            IsPLayAble = s.IsPLayable;
        
            Buckets = new ReactiveListNameSafe<Bucket>();
            foreach (var bucketState in s.bucketStates)
            {
                Buckets.Add(await Restore<Bucket>(bucketState));
            }

            BucketSelectors = new List<UtilityContainerSelector>();
            foreach (var utilityContainerSelectorState in s.bucketSelectorStates)
            {
                BucketSelectors.Add(await Restore<UtilityContainerSelector>(utilityContainerSelectorState));
            }
            
            DecisionSelectors = new List<UtilityContainerSelector>();
            foreach (var utilityContainerSelectorState in s.decisionSelectorStates)
            {
                DecisionSelectors.Add(await Restore<UtilityContainerSelector>(utilityContainerSelectorState));
            }

            CurrentBucketSelector = BucketSelectors
                                        .FirstOrDefault(d => d.GetName() == s.CurrentBucketSelectorName) ?? 
                                    BucketSelectors.FirstOrDefault();

            CurrentDecisionSelector = DecisionSelectors
                                          .FirstOrDefault(d => d.GetName() == s.CurrentDecisionSelectorName) ?? 
                                      DecisionSelectors.FirstOrDefault();

            var utilityScorers = AssetService.GetInstancesOfType<IUtilityScorer>();
            UtilityScorer = utilityScorers
                                .FirstOrDefault(u => u.GetName() == s.USName) ?? 
                            utilityScorers.FirstOrDefault();
        }
    }

    // protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    // {
    //     var state = (UaiState)s;
    //     Name = state.Name;
    //     Description = state.Description;
    //     IsPLayAble = state.IsPLayable;
    //     
    //     Buckets = new ReactiveListNameSafe<Bucket>();
    //     var bucketsLocal = await RestoreAbleService
    //         .GetAiObjectsSortedByIndex<Bucket>(CurrentDirectory + Consts.FolderName_Buckets, restoreDebug);
    //     Buckets.Add(RestoreAbleService.OrderByNames(state.Buckets, bucketsLocal));
    //
    //     BucketSelectors = await RestoreAbleService.GetUcs(CurrentDirectory + Consts.FolderName_BucketSelectors, restoreDebug);
    //     CurrentBucketSelector = BucketSelectors
    //         .FirstOrDefault(d => d.GetName() == state.CurrentBucketSelectorName) ?? 
    //             BucketSelectors.FirstOrDefault();
    //
    //     DecisionSelectors = await RestoreAbleService.GetUcs(CurrentDirectory + Consts.FolderName_DecisionSelectors, restoreDebug);
    //     CurrentDecisionSelector = DecisionSelectors
    //                 .FirstOrDefault(d => d.GetName() == state.CurrentDecisionSelectorName) ?? 
    //                               DecisionSelectors.FirstOrDefault();
    //
    //     var utilityScorers = AssetService.GetInstancesOfType<IUtilityScorer>();
    //     UtilityScorer = utilityScorers
    //                         .FirstOrDefault(u => u.GetName() == state.USName) ?? 
    //                     utilityScorers.FirstOrDefault();
    // }

    // protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    // {
    //     await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_UAI);
    //     await RestoreAbleService.SaveRestoreAblesToFile(Buckets.Values,path + "/" + Consts.FolderName_Buckets, persister);
    //     await RestoreAbleService.SaveRestoreAblesToFile(BucketSelectors,path + "/" + Consts.FolderName_BucketSelectors, persister);
    //     await RestoreAbleService.SaveRestoreAblesToFile(DecisionSelectors,path + "/" + Consts.FolderName_DecisionSelectors, persister);
    // }
    public override SingleFileState GetSingleFileState()
    {
        return new UaiSingleFileState(this);
    }
}

[Serializable]
public class UaiSingleFileState: AiObjectModelSingleFileState
{
    public bool IsPLayable;
    public string CurrentBucketSelectorName;
    public string CurrentDecisionSelectorName;
    public string USName;

    public List<UtilityContainerSelectorSingleFileState> bucketSelectorStates;
    public List<UtilityContainerSelectorSingleFileState> decisionSelectorStates;

    public List<BucketSingleFileState> bucketStates;

    public UaiSingleFileState() : base()
    {
    }

    public UaiSingleFileState(Uai o): base(o)
    {
        IsPLayable = o.IsPLayAble;

        bucketStates = new List<BucketSingleFileState>();
        foreach (var bucketsValue in o.Buckets.Values)
        {
            bucketStates.Add(bucketsValue.GetSingleFileState() as BucketSingleFileState);
        }

        bucketSelectorStates = new List<UtilityContainerSelectorSingleFileState>();
        foreach (var bucketSelector in o.BucketSelectors)
        {
            var state = bucketSelector.GetSingleFileState() as UtilityContainerSelectorSingleFileState;
            bucketSelectorStates.Add(state);
        }
        
        decisionSelectorStates = new List<UtilityContainerSelectorSingleFileState>();
        foreach (var decisionSelector in o.DecisionSelectors)
        {
            var state = decisionSelector.GetSingleFileState() as UtilityContainerSelectorSingleFileState;
            decisionSelectorStates.Add(state);
        }

        CurrentBucketSelectorName = o.CurrentBucketSelector?.GetName();
        CurrentDecisionSelectorName = o.CurrentDecisionSelector?.GetName();

        USName = o.UtilityScorer.GetName();
    }
}

// public class UaiState: AiObjectState
// {
//     public string Name;
//     public string Description;
//     public bool IsPLayable;
//     public string CurrentBucketSelectorName;
//     public string CurrentDecisionSelectorName;
//     public string USName;
//
//     public List<string> Buckets;
//
//     public UaiState() : base()
//     {
//     }
//
//     public UaiState(string name, string description, List<Bucket> buckets, Uai model): base(model)
//     {
//         Name = name;
//         Description = description;
//         IsPLayable = model.IsPLayAble;
//         
//         Buckets = RestoreAbleService.NamesToList(buckets);
//
//         CurrentBucketSelectorName = model.CurrentBucketSelector?.GetName();
//         CurrentDecisionSelectorName = model.CurrentDecisionSelector?.GetName();
//
//         USName = model.UtilityScorer.GetName();
//     }
// }