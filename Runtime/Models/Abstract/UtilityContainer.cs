using System;
using System.Collections.Generic;
using UniRx;
using UniRxExtension;
using System.Linq;

[Serializable]
public abstract class UtilityContainer : AiObjectModel
{
    protected int index { get; set; }
   
    protected string ContextAdress = "";
    private IDisposable considerationSub;
    private ReactiveListNameSafe<Consideration> considerations = new ReactiveListNameSafe<Consideration>();
    public ReactiveListNameSafe<Consideration> Considerations
    {
        get => considerations;
        set
        {
            considerations = value;
            if (considerations != null)
            {
                considerationSub?.Dispose();
                UpdateInfo();
                considerationSub = considerations.OnValueChanged
                    .Subscribe(_ => UpdateInfo());
            }
        }
    }

    private float lastCalculatedUtility;
    public float LastCalculatedUtility
    {
        get => lastCalculatedUtility;
        set
        {
            lastCalculatedUtility = value;
            ScoreModels[0].Value = value;
            lastUtilityChanged.OnNext(value);
        }
    }
    
    public IObservable<float> LastUtilityScoreChanged => lastUtilityChanged;
    private Subject<float> lastUtilityChanged = new Subject<float>();

    protected UtilityContainer() : base()
    {
        ScoreModels = new List<ScoreModel>();
        ScoreModels.Add(new ScoreModel("Score", 0f));
        //ScoreModels.Add(new ScoreModel("Normalized", 0f));

        considerationSub?.Dispose();
        UpdateInfo();
        considerationSub = considerations.OnValueChanged
            .Subscribe(_ => UpdateInfo());
    }

    protected override void SetBaseClone(AiObjectModel c)
    {
        var clone = c as UtilityContainer;
        base.SetBaseClone(clone);
        clone.Considerations = new ReactiveListNameSafe<Consideration>();
        foreach (var cons in Considerations.Values)
        {
            var cClone = cons.Clone() as Consideration;
            clone.Considerations.Add(cClone);
        }

        clone.ScoreModels = new List<ScoreModel>();
        foreach (var sm in ScoreModels)
        {
            var smClone = new ScoreModel(sm.Name, 0);
            clone.ScoreModels.Add(smClone);
        }
        clone.considerationSub?.Dispose();
        clone.UpdateInfo();
        clone.considerationSub = clone.considerations.OnValueChanged
            .Subscribe(_ => clone.UpdateInfo());
    }

    internal virtual void SetIndex(int value)
    {
        index = value;
        ContextAdress = index.ToString();
    }
    public override void SetContextAddress(string address)
    {
        base.SetContextAddress(address);
        foreach(var c in Considerations.Values)
        {
            c.SetContextAddress(ContextAddress + "C" + Considerations.Values.IndexOf(c));
        }
    }

    protected virtual float CalculateUtility(AiContext context)
    {
        return context.UtilityScorer.CalculateUtility(Considerations.Values, context);
    }

    internal float GetUtility(AiContext context)
    {
        MetaData.LastTickEvaluated = context.TickMetaData.TickCount;
        LastCalculatedUtility = CalculateUtility(context);
        if (float.IsNaN(LastCalculatedUtility))
        {
            LastCalculatedUtility = -1;
        }
        return LastCalculatedUtility;
    }

    public void SortConsiderations()
    {
        var setterList = new List<Consideration>();
        var performanceLists = new Dictionary<PerformanceTag, List<Consideration>>();
        foreach(var consideration in Considerations.Values)
        {
            if (consideration.IsSetter)
            {
                setterList.Add(consideration);
            }
            else
            {
                if (!performanceLists.ContainsKey(consideration.PerformanceTag))
                {
                    performanceLists.Add(consideration.PerformanceTag, new List<Consideration>());
                }
                performanceLists[consideration.PerformanceTag].Add(consideration);
            }
        }
        setterList = setterList.OrderBy(c => c.PerformanceTag).ToList();
        var result = setterList.ToList();

        foreach(PerformanceTag pTag in Enum.GetValues(typeof(PerformanceTag)))
        {
            if (!performanceLists.ContainsKey(pTag))
            {
                continue;
            }

            var booleans = new List<Consideration>();
            var nonBooleans = new List<Consideration>();
            foreach(var consideration in performanceLists[pTag])
            {
                if (consideration.GetType().IsSubclassOf(typeof(ConsiderationBoolean)))
                {
                    booleans.Add(consideration);
                } else
                {
                    nonBooleans.Add(consideration);
                }
            }
            booleans = booleans.OrderBy(c => c.PerformanceTag).ToList();
            nonBooleans = nonBooleans.OrderBy(c => c.PerformanceTag).ToList();
            result.AddRange(booleans);
            result.AddRange(nonBooleans);
        }

        Considerations.ChangeAll(result);
    }

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        considerationSub?.Dispose();
    }
}