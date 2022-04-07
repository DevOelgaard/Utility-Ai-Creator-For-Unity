using System;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using UniRx;
using System.Linq;

public abstract class Consideration : AiObjectModel
{
    private CompositeDisposable paramaterDisposables = new CompositeDisposable();
    public bool IsScorer { get; protected set; } = true;
    public bool IsModifier { get; protected set; } = false;
    public bool IsSetter { get; protected set; } = false;
    public List<Parameter> Parameters;
    private ResponseCurve currentResponseCurve;
    public ResponseCurve CurrentResponseCurve
    {
        get 
        { 
            if (currentResponseCurve == null)
            {
                currentResponseCurve = new ResponseCurve();
            }
            return currentResponseCurve; 
        }
        set
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

    protected Consideration() : base()
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

    public override string GetTypeDescription()
    {
        return "Consideration";
    }

    protected override AiObjectModel InternalClone()
    {
        var clone = (Consideration)Activator.CreateInstance(GetType());
        clone.Parameters = new List<Parameter>();
        foreach (var p in this.Parameters)
        {
            var c = p.Clone();
            clone.Parameters.Add(c);
        }

        clone.ScoreModels = new List<ScoreModel>();
        foreach (var sm in this.ScoreModels)
        {
            var c = new ScoreModel(sm.Name, 0);
            clone.ScoreModels.Add(c);
        }

        clone.PerformanceTag = this.PerformanceTag;
        clone.MinFloat = MinFloat.Clone();
        clone.MaxFloat = MaxFloat.Clone();

        clone.CurrentResponseCurve = (ResponseCurve)CurrentResponseCurve.Clone();
        return clone;
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

    protected virtual List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
    protected virtual float CalculateBaseScore(AiContext context)
    {
        return float.MinValue;
    }

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

        var state = (ConsiderationState)s;
        Name = state.Name;
        Description = state.Description;

        var minState = PersistenceAPI.Instance.LoadObjectsPathWithFilters<ParameterState>(CurrentDirectory+ Consts.FolderName_MinParameter, typeof(Parameter)).FirstOrDefault();
        if (minState.LoadedObject == null)
        {
            MinFloat = new Parameter(minState.ErrorMessage, minState.Exception.ToString());
        }
        else
        {
            MinFloat = Restore<Parameter>(minState.LoadedObject);
        }

        var maxState = PersistenceAPI.Instance.LoadObjectsPathWithFilters<ParameterState>(CurrentDirectory + Consts.FolderName_MaxParameter, typeof(Parameter)).FirstOrDefault();
        if (maxState.LoadedObject == null)
        {
            MaxFloat = new Parameter(maxState.ErrorMessage, maxState.Exception.ToString());
        }
        else
        {
            MaxFloat = Restore<Parameter>(maxState.LoadedObject);
        }

        var responseCurveState = PersistenceAPI.Instance.LoadObjectsPathWithFilters<ResponseCurveState>(CurrentDirectory + Consts.FolderName_ResponseCurves, typeof(ResponseCurve)).FirstOrDefault();
        if (responseCurveState != null)
        {
            if (responseCurveState.LoadedObject == null)
            {
                var error = (ResponseCurve)InstantiaterService.Instance.CreateInstance(responseCurveState.ModelType);
                error.Name = responseCurveState.ErrorMessage;
                error.Description = "Exception: " + responseCurveState.Exception.ToString();
                CurrentResponseCurve = error;
            } else
            {
                CurrentResponseCurve = Restore<ResponseCurve>(responseCurveState.LoadedObject);
            }
        }

        var parameters = RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
        Parameters = RestoreAbleService.SortByName(state.Parameters, parameters);

        PerformanceTag = (PerformanceTag)state.PerformanceTag;
        paramaterDisposables.Clear();

        MinFloat.OnValueChange
            .Subscribe(_ => CurrentResponseCurve.MinX = Convert.ToSingle(MinFloat.Value))
            .AddTo(paramaterDisposables);

        MaxFloat.OnValueChange
            .Subscribe(_ => CurrentResponseCurve.MaxX = Convert.ToSingle(MaxFloat.Value))
            .AddTo(paramaterDisposables);

        SetMinMaxForCurves();
        
        if (restoreDebug)
        {
            BaseScore = state.BaseScore;
            NormalizedScore = state.NormalizedScore;
        }
    }

    internal override RestoreState GetState()
    {
        return new ConsiderationState(Name,Description,Parameters, CurrentResponseCurve, MinFloat, MaxFloat, this);
    }

    protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        persister.SaveObject(state, path + "." + Consts.FileExtension_Consideration);
        foreach (var parameter in Parameters)
        {
            var subPath = path + "/" + Consts.FolderName_Parameters;
            parameter.SaveToFile(subPath, persister);
        }
        var rcPath = path + "/" + Consts.FolderName_ResponseCurves;
        CurrentResponseCurve.SaveToFile(rcPath, persister);

        var minPath = path + "/" + Consts.FolderName_MinParameter;
        MinFloat.SaveToFile(minPath, persister);
        var maxPath = path + "/" + Consts.FolderName_MaxParameter;
        MaxFloat.SaveToFile(maxPath, persister);
    }


    ~Consideration()
    {
        paramaterDisposables.Clear();
    }
}

[Serializable]
public class ConsiderationState: RestoreState
{
    public string Name;
    public string Description;
    public int PerformanceTag;
    public float BaseScore;
    public float NormalizedScore;
    public List<string> Parameters;

    public ConsiderationState() : base()
    {
    }

    public ConsiderationState(string name, string description, List<Parameter> parameters, ResponseCurve responseCurve, Parameter min, Parameter max, Consideration consideration): base(consideration)
    {
        Name = name;
        Description = description;
        PerformanceTag = (int)consideration.PerformanceTag;
        BaseScore = consideration.BaseScore;
        NormalizedScore = consideration.NormalizedScore;
        Parameters = RestoreAbleService.NamesToList(parameters);
    }
}