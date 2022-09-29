using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public abstract class ResponseFunction: AiObjectModel
{
    private readonly CompositeDisposable parametersChangedDisposable = new CompositeDisposable();
    public int rcIndex = -1;
    private ParamFloat max;
    public ParamFloat Max => max ??= ParameterContainer.GetParamFloat("Max");

    public IObservable<bool> OnParametersChanged => onParametersChanged;
    protected readonly Subject<bool> onParametersChanged = new Subject<bool>();

    private bool Inverse => ParameterContainer.GetParamBool("Inverse").Value;
    protected ResponseFunction()
    {
        DebugService.Log("TT! Creating no name", this);

        InitParameters();
        BaseAiObjectType = typeof(ResponseFunction);
    }

    protected ResponseFunction(string name)
    {
        DebugService.Log("TT! Creating RF: " + Guid, this);

        Name = name;
        InitParameters();
        BaseAiObjectType = typeof(ResponseFunction);
    }

    private void InitParameters()
    {
        AddParameter("Inverse", false);
        AddParameter("Max", 1f);
    }

    public override void Initialize()
    {
        base.Initialize();
        SubscribeToParameters();
    }

    protected override void OnCloneComplete()
    {
        base.OnCloneComplete();
        Initialize();
    }

    protected override void OnRestoreComplete()
    {
        base.OnRestoreComplete();
        Initialize();
    }

    protected override AiObjectModel InternalClone()
    {
        var clone = AssetService.GetInstanceOfType<ResponseFunction>(GetType().ToString());
        DebugService.Log("TT! cloning Original Guid: " + Guid + " Clone Guid: " + clone.Guid, this,Thread.CurrentThread);

        // var clone = (ResponseFunction)AiObjectFactory.CreateInstance(GetType());
        if (clone.Guid == Guid)
        {
            DebugService.LogError("Clone and original are the same", this);
        }

        DebugService.Log("RF cloning complete Name: " + Name, this);
        return clone;
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

    protected abstract float CalculateResponseInternal(float x);

    private void SubscribeToParameters()
    {
        parametersChangedDisposable.Clear();
        DebugService.Log("TT! Subscribing to Parameters.count: " + Parameters.Count + " Guid: " + Guid, this,Thread.CurrentThread);
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
    
    public override SingleFileState GetSingleFileState()
    {
        return new ResponseFunctionSingleFileState(this);
    }
    
    protected override async Task RestoreInternalFromFile(SingleFileState state)
    {
    }


    ~ResponseFunction()
    {
        DebugService.Log("TT! Destroying", this);
        parametersChangedDisposable.Clear();
    }
}

// [Serializable]
// public class ResponseFunctionState : AiObjectState
// {
//     public string Name;
//     public string Description;
//     public int RcIndex;
//     public List<string> Parameters;
//
//     public ResponseFunctionState() : base()
//     {
//     }
//
//     public ResponseFunctionState(ResponseFunction responseFunction) : base(responseFunction)
//     {
//         Name = responseFunction.Name;
//         Description = responseFunction.Description;
//         RcIndex = responseFunction.rcIndex;
//         Parameters = RestoreAbleService.NamesToList(responseFunction.Parameters.ToList());
//
//     }
// }

[Serializable]
public class ResponseFunctionSingleFileState : AiObjectModelSingleFileState
{
    public ResponseFunctionSingleFileState()
    {
    }

    public ResponseFunctionSingleFileState(ResponseFunction rF): base(rF)
    {
    }
}
