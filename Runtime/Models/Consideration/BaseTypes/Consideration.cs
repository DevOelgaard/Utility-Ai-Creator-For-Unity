using System;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using UniRx;
using System.Linq;
using System.Threading.Tasks;

public abstract class Consideration : AiObjectModel
{
    private readonly CompositeDisposable parameterDisposables = new CompositeDisposable();
    public virtual bool IsScorer { get; protected set; } = true;
    public bool IsModifier { get; protected set; } = false;
    public virtual bool IsSetter { get; protected set; } = false;
    // public List<Parameter> Parameters;
    private ResponseCurve currentResponseCurve;
    public ResponseCurve CurrentResponseCurve
    {
        get 
        { 
            if (currentResponseCurve == null)
            {
                currentResponseCurve = (ResponseCurve)AiObjectFactory.CreateInstance(typeof(ResponseCurve));
            }
            return currentResponseCurve; 
        }
        set
        {
            if(value == currentResponseCurve) return;
            currentResponseCurve = value;
            onResponseCurveChanged.OnNext(currentResponseCurve);
        }
    }
    public IObservable<ResponseCurve> OnResponseCurveChanged => onResponseCurveChanged;
    private readonly Subject<ResponseCurve> onResponseCurveChanged = new Subject<ResponseCurve>();
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
        
        ScoreModels = new List<ScoreModel>
        {
            new ScoreModel("Base", 0f),
            new ScoreModel("Score", 0f)
        };

        MinFloat.OnValueChange
            .Subscribe(_ => CurrentResponseCurve.MinX = Convert.ToSingle(MinFloat.Value))
            .AddTo(parameterDisposables);

        MaxFloat.OnValueChange
            .Subscribe(_ => CurrentResponseCurve.MaxX = Convert.ToSingle(MaxFloat.Value))
            .AddTo(parameterDisposables);

        SetMinMaxForCurves();
        BaseAiObjectType = typeof(Consideration);
    }

    public override void Initialize()
    {
        base.Initialize();
        PerformanceTag = GetPerformanceTag();
    }

    public override string GetTypeDescription()
    {
        return GetType().ToString();
    }

    protected override AiObjectModel InternalClone()
    {
        var clone = (Consideration)AiObjectFactory.CreateInstance(GetType());
        // foreach (var p in this.Parameters)
        // {
        //     var c = p.Clone();
        //     clone.AddParameter(c);
        // }

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

    protected override string GetNameFormat(string name)
    {
        return name;
    }

    protected virtual PerformanceTag GetPerformanceTag()
    {
        return PerformanceTag.Normal;
    }

    protected abstract float CalculateBaseScore(IAiContext context);

    public virtual float CalculateScore(IAiContext context)
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

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        await base.RestoreInternalAsync(s, restoreDebug);

        var state = (ConsiderationState)s;
            Name = state.Name;
            Description = state.Description;

            var minSates = await PersistenceAPI.Instance
                .LoadObjectsOfTypeAsync<ParameterState>(CurrentDirectory + Consts.FolderName_MinParameter,
                    typeof(Parameter));
            var minState = minSates.FirstOrDefault();
            
            if (minState.LoadedObject == null)
            {
                MinFloat = new Parameter(minState.ErrorMessage, minState.Exception.ToString());
            }
            else
            {
                MinFloat = await Restore<Parameter>(minState.LoadedObject);
            }

            var maxStates = await PersistenceAPI.Instance
                .LoadObjectsOfTypeAsync<ParameterState>(CurrentDirectory + Consts.FolderName_MaxParameter,
                    typeof(Parameter));
            var maxState = maxStates.FirstOrDefault();
            
            if (maxState.LoadedObject == null)
            {
                MaxFloat = new Parameter(maxState.ErrorMessage, maxState.Exception.ToString());
            }
            else
            {
                MaxFloat = await Restore<Parameter>(maxState.LoadedObject);
            }

            var rC = await RestoreAbleService
                .GetAiObjectsSortedByIndex<ResponseCurve>(CurrentDirectory + Consts.FolderName_ResponseCurves, restoreDebug);
            CurrentResponseCurve = rC.First();
                

            // var parameters = await RestoreAbleService
            //     .GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);

            PerformanceTag = (PerformanceTag)state.PerformanceTag;
            parameterDisposables.Clear();

            MinFloat.OnValueChange
                .Subscribe(_ => CurrentResponseCurve.MinX = Convert.ToSingle(MinFloat.Value))
                .AddTo(parameterDisposables);

            MaxFloat.OnValueChange
                .Subscribe(_ => CurrentResponseCurve.MaxX = Convert.ToSingle(MaxFloat.Value))
                .AddTo(parameterDisposables);

            SetMinMaxForCurves();
            
            if (restoreDebug)
            {
                BaseScore = state.BaseScore;
                NormalizedScore = state.NormalizedScore;
            }
    }

    internal override RestoreState GetState()
    {
        return new ConsiderationState(Name,Description,Parameters.ToList(), CurrentResponseCurve, MinFloat, MaxFloat, this);
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_Consideration);
        //await RestoreAbleService.SaveRestoreAblesToFile(Parameters,path + "/" + Consts.FolderName_Parameters, persister);

        var rcPath = path + "/" + Consts.FolderName_ResponseCurves;
        await CurrentResponseCurve.SaveToFile(rcPath, persister);

        var minPath = path + "/" + Consts.FolderName_MinParameter;
        await MinFloat.SaveToFile(minPath, persister);
        var maxPath = path + "/" + Consts.FolderName_MaxParameter;
        await MaxFloat.SaveToFile(maxPath, persister);
    }

    ~Consideration()
    {
        parameterDisposables.Clear();
    }
}

[Serializable]
public class ConsiderationState: AiObjectState
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
        PerformanceTag = Convert.ToInt32(consideration.PerformanceTag);
        BaseScore = consideration.BaseScore;
        NormalizedScore = consideration.NormalizedScore;
        Parameters = RestoreAbleService.NamesToList(parameters);
    }
}