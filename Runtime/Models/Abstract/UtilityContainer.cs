using System;
using System.Collections.Generic;
using UniRx;
using UniRxExtension;

[Serializable]
public abstract class UtilityContainer : AiObjectModel
{
    protected int index { get; set; }
   
    protected string ContextAdress = "";
    private IDisposable considerationSub;
    private ReactiveListNameSafe<Consideration> considerations = new ReactiveListNameSafe<Consideration>();
    internal ReactiveListNameSafe<Consideration> Considerations
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

    protected override void ClearSubscriptions()
    {
        base.ClearSubscriptions();
        considerationSub?.Dispose();
    }
}