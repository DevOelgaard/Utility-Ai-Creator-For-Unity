using System;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using UniRx;
using System.Linq;

public abstract class Consideration : AiObjectModel
{
    private CompositeDisposable paramaterDisposables = new CompositeDisposable();

    public List<Parameter> Parameters;
    private ResponseCurve currentResponseCurve;
    public ResponseCurve CurrentResponseCurve
    {
        get 
        { 
            if (currentResponseCurve == null)
            {
                currentResponseCurve = new ResponseCurve();
                //currentResponseCurve = AssetDatabaseService.GetInstancesOfType<ResponseCurve>()
                //    .FirstOrDefault();
            }
            return currentResponseCurve; 
        }
        protected set
        {
            currentResponseCurve = value;
            onResponseCurveChanged.OnNext(currentResponseCurve);
        }
    }
    public IObservable<ResponseCurve> OnResponseCurveChanged => onResponseCurveChanged;
    private Subject<ResponseCurve> onResponseCurveChanged = new Subject<ResponseCurve>();
    public PerformanceTag PerformanceTag;
    public float BaseScore
    {
        get => ScoreModels[0].Value;
        set => ScoreModels[0].Value = value;
    }
    public IObservable<float> BaseScoreChanged => ScoreModels[0].OnValueChanged;
    public float NormalizedScore
    {
        get => ScoreModels[1].Value;
        set => ScoreModels[1].Value = value;
    }
    public IObservable<float> NormalizedScoreChanged => ScoreModels[1].OnValueChanged;

    public Parameter MinFloat = new Parameter("Min", 0f);
    public Parameter MaxFloat = new Parameter("Max", 1f);

    protected Consideration()
    {
        Parameters =  new List<Parameter>(GetParameters());
        ScoreModels = new List<ScoreModel>();
        ScoreModels.Add(new ScoreModel("Base", 0f));
        ScoreModels.Add(new ScoreModel("Score", 0f));
        PerformanceTag = GetPerformanceTag();

        MinFloat.OnValueChange
            .Subscribe(_ => CurrentResponseCurve.MinX = Convert.ToSingle(MinFloat.Value))
            .AddTo(paramaterDisposables);

        MaxFloat.OnValueChange
            .Subscribe(_ => CurrentResponseCurve.MaxX = Convert.ToSingle(MaxFloat.Value))
            .AddTo(paramaterDisposables);

        SetMinMaxForCurves();
    }

    private void SetMinMaxForCurves()
    {
        CurrentResponseCurve.MinX = Convert.ToSingle(MinFloat.Value);
        CurrentResponseCurve.MaxX = Convert.ToSingle(MaxFloat.Value);
    }

    public override string GetNameFormat(string name)
    {
        return name;
    }

    protected virtual PerformanceTag GetPerformanceTag()
    {
        return PerformanceTag.Normal;
    }

    protected abstract List<Parameter> GetParameters();
    protected abstract float CalculateBaseScore(AiContext context);

    public virtual float CalculateScore(AiContext context)
    {
        BaseScore = CalculateBaseScore(context);
        if (BaseScore < Convert.ToSingle(MinFloat.Value))
        {
            return BaseScoreBelowMinValue();
        }
        else if (BaseScore > Convert.ToSingle(MaxFloat.Value))
        {
            return BaseScoreAboveMaxValue();
        }
        //var normalizedBaseScore = Normalize(BaseScore);
        var response = CurrentResponseCurve.CalculateResponse(BaseScore);
        NormalizedScore = Mathf.Clamp(response, 0f, 1f);

        return NormalizedScore;
    }



    protected virtual float BaseScoreBelowMinValue()
    {
        return 0;
    }

    protected virtual float BaseScoreAboveMaxValue()
    {
        return 1;
    }

    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {

        var sw = new System.Diagnostics.Stopwatch();
        var swTotal = new System.Diagnostics.Stopwatch();
        sw.Start();
        swTotal.Start();
        var state = (ConsiderationState)s;
        Name = state.Name;
        Description = state.Description;
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons Cast state");
        sw.Restart();

        MinFloat = Parameter.Restore<Parameter>(state.Min);
        MaxFloat = Parameter.Restore<Parameter>(state.Max);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons Set MinMax");
        sw.Restart();
        if (state.ResponseCurveState != null)
        {
            sw.Restart();
            CurrentResponseCurve = ResponseCurve.Restore<ResponseCurve>(state.ResponseCurveState, restoreDebug);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons ResponseCurves");

        }

        Parameters = new List<Parameter>();
        foreach (var pState in state.Parameters)
        {
            sw.Restart();
            var parameter = Parameter.Restore<Parameter>(pState, restoreDebug);
            Parameters.Add(parameter);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons Parameters");

        }
        sw.Restart();
        PerformanceTag = (PerformanceTag)state.PerformanceTag;
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons PerformanceTag");

        sw.Restart();
        paramaterDisposables.Clear();
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons Clear disposables");

        sw.Restart();

        MinFloat.OnValueChange
            .Subscribe(_ => CurrentResponseCurve.MinX = Convert.ToSingle(MinFloat.Value))
            .AddTo(paramaterDisposables);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons Subscribe");
        sw.Restart();

        MaxFloat.OnValueChange
            .Subscribe(_ => CurrentResponseCurve.MaxX = Convert.ToSingle(MaxFloat.Value))
            .AddTo(paramaterDisposables);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons Subscribe");
        sw.Restart();

        SetMinMaxForCurves();
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons SetMinMaxCurves");

        sw.Restart();

        if (restoreDebug)
        {
            BaseScore = state.BaseScore;
            NormalizedScore = state.NormalizedScore;
        }
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "RestoreInternal Cons IfRestoreDebug");
        TimerService.Instance.LogCall(swTotal.ElapsedMilliseconds, "RestoreInternal Cons Consideration Total");

    }

    internal override AiObjectModel Clone()
    {
        var state = GetState();
        var clone = Restore<Consideration>(state);
        return clone;
    }

    internal override RestoreState GetState()
    {
        return new ConsiderationState(Name,Description,Parameters, CurrentResponseCurve, MinFloat, MaxFloat, this);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState();
        persister.SaveObject(state, path);
    }


    ~Consideration()
    {
        paramaterDisposables.Clear();
    }
}

[Serializable]
public class ConsiderationState: RestoreState
{
    public List<ParameterState> Parameters;
    public ResponseCurveState ResponseCurveState;
    public string Name;
    public string Description;
    public ParameterState Min;
    public ParameterState Max;
    public int PerformanceTag;
    public float BaseScore;
    public float NormalizedScore;
    public ConsiderationState() : base()
    {
    }

    public ConsiderationState(string name, string description, List<Parameter> parameters, ResponseCurve responseCurve, Parameter min, Parameter max, Consideration consideration): base(consideration)
    {
        Name = name;
        Description = description;
        ResponseCurveState = responseCurve.GetState() as ResponseCurveState;
        Min = min.GetState() as ParameterState;
        Max = max.GetState() as ParameterState;

        Parameters = new List<ParameterState>();
        foreach (var parameter in parameters)
        {
            Parameters.Add(parameter.GetState() as ParameterState);
        }

        PerformanceTag = (int)consideration.PerformanceTag;

        BaseScore = consideration.BaseScore;
        NormalizedScore = consideration.NormalizedScore;
    }
}