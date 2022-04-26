using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public abstract class ResponseFunction: AiObjectModel
{
    private CompositeDisposable parametersChangedDisposable = new CompositeDisposable();
    public int rcIndex = -1;
    private Parameter max;
    protected Parameter Max => max ??= GetParameter("Max");

    public IObservable<bool> OnParametersChanged => onParametersChanged;
    private readonly Subject<bool> onParametersChanged = new Subject<bool>();

    private bool Inverse => (bool)GetParameter("Inverse").Value;

    protected ResponseFunction()
    {
        AddParameter(new Parameter("Inverse", false));
        AddParameter(new Parameter("Max", 1f));
        BaseAiObjectType = typeof(ResponseFunction);
    }

    protected ResponseFunction(string name)
    {
        Name = name;
        AddParameter(new Parameter("Inverse", false ));
        AddParameter(new Parameter("Max", 1f));
        BaseAiObjectType = typeof(ResponseFunction);
    }

    public override void Initialize()
    {
        base.Initialize();
        SubscribeToParameters();
    }

    protected override AiObjectModel InternalClone()
    {
        var clone = (ResponseFunction)AiObjectFactory.CreateInstance(GetType());

        return clone;
    }

    protected override string GetFileName()
    {
        return rcIndex + " - " + base.GetFileName();
    }

    public virtual float CalculateResponse(float x, float prevResult, float maxY)
    {
        var result = 0f;
        if (Inverse)
        {
            result = 1-CalculateResponseInternal(x);
        } else
        {
            result = CalculateResponseInternal(x);
        }
        var factor = Convert.ToSingle(Max.Value) / maxY;
        result *= factor;
        return result + prevResult;
    }

    private float Normalize(float value, float min, float tempMax)
    {
        var factor = (tempMax - min) / tempMax;
        var x = value * factor;// * ResultFactor;
        return x;
    }

    protected abstract float CalculateResponseInternal(float x);

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var state = (ResponseFunctionState) s;
        Name = state.Name;

        var parameters = await RestoreAbleService
                .GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
        foreach (var parameter in parameters)
        {
            AddParameter(parameter);
        }
    }

    private void SubscribeToParameters()
    {
        foreach (var parameter in Parameters)
        {
            DebugService.Log("Subscribing to parameter: " +parameter.Name, this );
            parameter.OnValueChange
                .Subscribe(_ =>
                {
                    DebugService.Log("Sending on parameter changed for: " + parameter.Name, this);
                    onParametersChanged.OnNext(true);
                })
                .AddTo(parametersChangedDisposable);
        }
    }


    internal override RestoreState GetState()
    {
        return new ResponseFunctionState(this);
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_ResponseFunction);
        await RestoreAbleService.SaveRestoreAblesToFile(Parameters,path + "/" + Consts.FolderName_Parameters, persister);
    }

    ~ResponseFunction()
    {
        DebugService.Log("Destroying", this);
        parametersChangedDisposable.Clear();
    }
}

[Serializable]
public class ResponseFunctionState : RestoreState
{
    public string Name;
    public string Description;
    public int RcIndex;
    public List<string> Parameters;

    public ResponseFunctionState() : base()
    {
    }

    public ResponseFunctionState(ResponseFunction responseFunction) : base(responseFunction)
    {
        Name = responseFunction.Name;
        Description = responseFunction.Description;
        RcIndex = responseFunction.rcIndex;
        Parameters = RestoreAbleService.NamesToList(responseFunction.Parameters.ToList());

    }
}