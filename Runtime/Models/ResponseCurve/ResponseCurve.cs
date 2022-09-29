using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

// https://forum.unity.com/threads/draw-a-line-from-a-to-b.698618/
public class ResponseCurve: AiObjectModel
{
    private readonly Dictionary<ParamFloat, IDisposable> segmentDisposables = new Dictionary<ParamFloat, IDisposable>();
    private readonly Dictionary<ResponseFunction, IDisposable> responseFunctionDisposables = new Dictionary<ResponseFunction, IDisposable>();
    private float minY = 0.0f;
    private float maxY = 1.0f;
    private float minX = 0.0f;
    private float maxX = 1.0f;
    private bool isInversed = false;

    public bool IsInversed
    {
        get => isInversed;
        set
        {
            isInversed = value;
            onCurveValueChanged.OnNext(true);
        }
    }

    private List<ResponseFunction> responseFunctions;

    public List<ResponseFunction> ResponseFunctions
    {
        get
        {
            if (responseFunctions == null)
            {
                responseFunctions = new List<ResponseFunction>();
            }

            return responseFunctions;
        }
    }
    public List<ParamFloat> Segments = new List<ParamFloat>();

    public IObservable<bool> OnParametersChanged => onParametersChanged;
    private readonly Subject<bool> onParametersChanged = new Subject<bool>();
    public IObservable<bool> OnCurveValueChanged => onCurveValueChanged;
    private readonly Subject<bool> onCurveValueChanged = new Subject<bool>();
    
    public IObservable<bool> OnFunctionsChanged => onFunctionsChanged;
    private readonly Subject<bool> onFunctionsChanged = new Subject<bool>();
    private Guid guid;
    public ResponseCurve()
    {
        DebugService.Log("T! Creating",this);
        BaseAiObjectType = typeof(ResponseCurve);
    }

    public override void Initialize()
    {
        DebugService.Log("T! Initializing",this);
        base.Initialize();
        guid = Guid.NewGuid();
        DebugService.Log("Subscribing to change Guid: " + guid, this);
        OnCurveValueChanged.Subscribe(_ => DebugService.Log("CurveValue Changed", this))
            .AddTo(disposables);
        DebugService.Log("Created Guid: " + guid + " Name: " + Name, this);
    }

    #region ResponseFunctions

    public void AddResponseFunction(ResponseFunction newFunction, bool updateSegments = true)
    {
        DebugService.Log("TT! Adding RF: " + newFunction.Guid, this);
        var previousFunction = ResponseFunctions.LastOrDefault();

        if (previousFunction != null && updateSegments) // Not First function
        {
            var previousSegment = Segments.LastOrDefault();
            var segmentValue = 0f;
            if (previousSegment != null)
            {
                segmentValue = (MaxX - Convert.ToSingle(previousSegment.Value)) / 2 + Convert.ToSingle(previousSegment.Value);

            } else
            {
                segmentValue = (MaxX - MinX) / 2;
            }
            var newSegment = new ParamFloat(Segments.Count.ToString(), segmentValue);
            AddSegment(newSegment);
        }

        DebugService.Log("Adding response function: " + newFunction.Name, this );
        ResponseFunctions.Add(newFunction);
        var sub = newFunction.OnParametersChanged
            .Subscribe(_ => onCurveValueChanged.OnNext(true));
        
        responseFunctionDisposables.Add(newFunction,sub);
        onFunctionsChanged.OnNext(true);
        UpdateResponseFunctionIndexes();
    }
    private void UpdateResponseFunctionIndexes()
    {
        foreach (var responseFunction in ResponseFunctions)
        {
            responseFunction.rcIndex = ResponseFunctions.IndexOf(responseFunction);
        }
    }

    internal void UpdateFunction(ResponseFunction oldFunction, ResponseFunction newFunction)
    {
        var oldFunctionIndex = ResponseFunctions.IndexOf(oldFunction);
        ResponseFunctions[oldFunctionIndex] = newFunction;
        var sub = newFunction.OnParametersChanged
            .Subscribe(_ => onCurveValueChanged.OnNext(true));
        responseFunctionDisposables.Add(newFunction,sub);
        if (responseFunctionDisposables.ContainsKey(oldFunction))
        {
            responseFunctionDisposables[oldFunction].Dispose();
            responseFunctionDisposables.Remove(oldFunction);
        }
        
        onFunctionsChanged.OnNext(true);
    }

    public void RemoveResponseFunction(ResponseFunction responseFunction)
    {
        DebugService.Log("TT! Removing RF: " + responseFunction.Guid, this);
        var functionIndex = ResponseFunctions.IndexOf(responseFunction);
        var removeIndex = functionIndex - 1;
        if (removeIndex < 0) // Removing first or only function
        {
            // if (ResponseFunctions.Count <= 1)
            // {
            //     throw new Exception("Can't remove the last Response function");
            // }
            ResponseFunctions.Remove(responseFunction);
            
            if (Segments.Count > 0)
            {
                RemoveSegment(Segments[0]);
            }
        } 
        else if (functionIndex == Segments.Count) // Removing last function
        {
            ResponseFunctions.Remove(responseFunction);
            RemoveSegment(Segments[removeIndex]);
        }
        else
        {
            RemoveSegment(Segments[removeIndex]);
            ResponseFunctions.Remove(responseFunction);
        }

        if (responseFunctionDisposables.ContainsKey(responseFunction))
        {
            responseFunctionDisposables[responseFunction].Dispose();
            responseFunctionDisposables.Remove(responseFunction);
        }
        onFunctionsChanged.OnNext(true);
        UpdateResponseFunctionIndexes();
    }

    #endregion

    #region Segments

    private void AddSegment(ParamFloat newSegment)
    {
        var sub = newSegment.OnValueChange
            .Subscribe(_ => onCurveValueChanged.OnNext(true));
        segmentDisposables.Add(newSegment,sub);
        Segments.Add(newSegment);
    }

    private void SetSegmentValue(float value, int index)
    {
        if (index > Segments.Count)
        {
            DebugService.LogError("Can't access segment at index: " + index + " Segments.Count: " + Segments.Count, this + " " + Name);
        }
        else
        {
            DebugService.Log("Setting Segment value at index: " + index + " to: " + value, this + " " + Name);
            Segments[index].Value = value;
        }
        // onCurveValueChanged.OnNext(true);
    }

    private void RemoveSegment(ParamFloat segment)
    {
        if (segmentDisposables.ContainsKey(segment))
        {
            segmentDisposables[segment].Dispose();
            segmentDisposables.Remove(segment);
        }

        Segments.Remove(segment);
    }
    
    private float GetSegmentMin(float x)
    {
        var segmentWithLowerValue = Segments
            .LastOrDefault(s => Convert.ToSingle(s.Value) < x);

        if (segmentWithLowerValue == null)
        {
            return MinY;
        } else
        {
            return Convert.ToSingle(segmentWithLowerValue.Value);
        }
    }

    private float GetSegmentMax(float x)
    {
        var segmentWithHigherValue = Segments
            .FirstOrDefault(s => Convert.ToSingle(s.Value) >= x);

        if (segmentWithHigherValue == null)
        {
            return maxX;
        } else
        {
            return Convert.ToSingle(segmentWithHigherValue.Value);
        }
    }

    #endregion

    #region Calculate

    public float CalculateResponse(float x)
    {
        if (ResponseFunctions.Count <= 0)
        {
            var cast = (LinearFunction) AiObjectFactory.CreateInstance(typeof(LinearFunction));
            DebugService.Log("TT! Creating new RF Rf Guid: " + cast.Guid,this ,Thread.CurrentThread);
            AddResponseFunction(cast);
        }
        var result = 0f;

        var validSegments = Segments
            .Where(s => Convert.ToSingle(s.Value) < x)
            .ToList();
        var normalized = 0f;
        var min = GetSegmentMin(x);
        var max = GetSegmentMax(x);

        if (validSegments.Count == 0)
        {
            normalized = Normalize(x, min, max);
            result = ResponseFunctions[0].CalculateResponse(normalized,minY, maxY);
        } else
        {
            var indexOfLastFunction = validSegments.Count;
            var previousMax = 0f;
            for (var i = 0; i < indexOfLastFunction; i++)
            {
                // Getting max of previous functions
                var currentMax = Convert.ToSingle(validSegments[i].Value);
                normalized = Normalize(x, previousMax, currentMax);
                previousMax = currentMax;
                result = ResponseFunctions[i].CalculateResponse(normalized, result,maxY);
            }

            normalized = Normalize(x, min, max);
            result = ResponseFunctions[indexOfLastFunction].CalculateResponse(normalized, result, maxY);
        }
        result = Mathf.Clamp(result,minY,maxY);

        return IsInversed ? 1 - result : result;
    }

    private float Normalize(float value, float min, float max)
    {
        var x = (value - min) / (max - min);// * ResultFactor;
        x = Mathf.Clamp(x, 0, 1);
        return x;
    }

    #endregion

    #region Fields

    public float MinY
    {
        get => minY;
        set
        {
            minY = value;
            onParametersChanged.OnNext(true);
        }
    }
    public float MaxY
    {
        get => maxY;
        set
        {
            maxY = value;
            onParametersChanged.OnNext(true);
        }
    }
    public float MinX
    {
        get => minX; 
        set
        {
            var oldRange = MaxX - MinX;
            minX = value;
            UpdateMinMax(oldRange);
            onParametersChanged.OnNext(true);
        }
 
    }
    public float MaxX
    {
        get => maxX;
        set
        {
            var oldRange = MaxX - MinX;
            maxX = value;
            UpdateMinMax(oldRange);
            onParametersChanged.OnNext(true);
        }
    }

    private void UpdateMinMax(float oldRange)
    {
        var factor = (MaxX-MinX) / oldRange;

        foreach (var segment in Segments)
        {
            var v = Convert.ToSingle(segment.Value);
            segment.Value = v * factor;
        }
    }

    #endregion
   
    protected override AiObjectModel InternalClone()
    {
        DebugService.Log("T! Cloning",this);
        var clone = (ResponseCurve)AiObjectFactory.CreateInstance(GetType());
        var state = GetSingleFileState() as ResponseCurveSingleFileState;
        AsyncHelpers.RunSync(async () => await clone.SetSavedValues(state));
        return clone;
    }
    // internal override RestoreState GetState()
    // {
    //     return new ResponseCurveState(Name, MinY, MaxY, Segments, this);
    // }
    //
    // protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    // {
    //     await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_ResponseCurve);
    //     await RestoreAbleService.SaveRestoreAblesToFile(Segments,path + "/" + Consts.FolderName_Segments, persister);
    //     await RestoreAbleService.SaveRestoreAblesToFile(ResponseFunctions,path + "/" + Consts.FolderName_ResponseFunctions, persister);
    // }
    //
    // protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    // {
    //     await base.RestoreInternalAsync(s, restoreDebug);
    //     var tempRestoreFunctions = await RestoreAbleService
    //         .GetAiObjectsSortedByIndex<ResponseFunction>(CurrentDirectory + Consts.FolderName_ResponseFunctions, restoreDebug);
    //     var tempSegments = await RestoreAbleService
    //         .GetParameters(CurrentDirectory + Consts.FolderName_Segments, restoreDebug);
    //     
    //     SetBaseValues(s as ResponseCurveState, tempRestoreFunctions, tempSegments);
    // }
    //
    // private void SetBaseValues(ResponseCurveState state, List<ResponseFunction> rFs, List<Parameter> segments)
    // {
    //     MinY = state.MinY;
    //     MaxY = state.MaxY;
    //     MinX = state.MinX;
    //     MaxX = state.MaxX;
    //     isInversed = state.IsInversed;
    //     
    //     foreach (var responseFunction in rFs)
    //     {
    //         AddResponseFunction(responseFunction, true);
    //     }
    //     foreach (var seg in segments)
    //     {
    //         SetSegmentValue(Convert.ToSingle(seg.Value),segments.IndexOf(seg));
    //     }
    // }

    protected override async Task RestoreInternalFromFile(SingleFileState state)
    {
        await SetSavedValues(state as ResponseCurveSingleFileState);
    }

    private async Task SetSavedValues(ResponseCurveSingleFileState state)
    {
        var s = state as ResponseCurveSingleFileState;
        MinY = s.MinY;
        MaxY = s.MaxY;
        MinX = s.MinX;
        MaxX = s.MaxX;
        isInversed = s.IsInversed;
        
        var tempRFs = new List<ResponseFunction>();
        foreach (var rfState in s.responseFunctions)
        {
            var rF = await Restore<ResponseFunction>(rfState);
            tempRFs.Add(rF);
        }
        
        var tempSegments = new List<Parameter>();
        foreach (var segmentState in s.segments)
        {
            var segment = await RestoreAble.Restore<Parameter>(segmentState);
            tempSegments.Add(segment);
        }
        
        foreach (var responseFunction in tempRFs)
        {
            AddResponseFunction(responseFunction, true);
        }
        foreach (var seg in tempSegments)
        {
            SetSegmentValue(Convert.ToSingle(seg.Value),tempSegments.IndexOf(seg));
        }
    }
    public override SingleFileState GetSingleFileState()
    {
        return new ResponseCurveSingleFileState(this);
    }

    ~ResponseCurve()
    {
        DebugService.Log("Destroying Guid: " + guid + " Name: " + Name, this);
        foreach(var disposable in segmentDisposables)
        {
            disposable.Value.Dispose();
        }
        foreach(var disposable in responseFunctionDisposables)
        {
            disposable.Value.Dispose();
        }
    }


}

// [Serializable]
// public class ResponseCurveState: AiObjectState
// {
//     public string Name;
//     public string Description;
//     public float MinY;
//     public float MaxY;
//     public float MinX;
//     public float MaxX;
//     public bool IsInversed;
//
//     public List<string> segments;
//
//
//     public ResponseCurveState(): base()
//     {
//     }
//
//     public ResponseCurveState(string name, float minY, float maxY, List<Parameter> segments, ResponseCurve responseCurveModel): base(responseCurveModel)
//     {
//         Name = name;
//         Description = responseCurveModel.Description;
//         MinY = minY;
//         MaxY = maxY;
//         MinX = responseCurveModel.MinX;
//         MaxX = responseCurveModel.MaxX;
//         IsInversed = responseCurveModel.IsInversed;
//     }
// }

[Serializable]
public class ResponseCurveSingleFileState : AiObjectModelSingleFileState
{
    public float MinY;
    public float MaxY;
    public float MinX;
    public float MaxX;
    public bool IsInversed;
    public List<ResponseFunctionSingleFileState> responseFunctions;
    public List<ParameterState> segments;

    public ResponseCurveSingleFileState(): base()
    {
    }

    public ResponseCurveSingleFileState(ResponseCurve responseCurveModel): base(responseCurveModel)
    {
        MinY = responseCurveModel.MinY;
        MaxY = responseCurveModel.MaxY;
        MinX = responseCurveModel.MinX;
        MaxX = responseCurveModel.MaxX;
        IsInversed = responseCurveModel.IsInversed;
        responseFunctions = new List<ResponseFunctionSingleFileState>();
        foreach (var responseFunction in responseCurveModel.ResponseFunctions)
        {
            responseFunctions.Add((ResponseFunctionSingleFileState)responseFunction.GetSingleFileState());
        }
        segments = new List<ParameterState>();
        foreach (var segment in responseCurveModel.Segments)
        {
            segments.Add((ParameterState)segment.GetState());
        }
    }
}