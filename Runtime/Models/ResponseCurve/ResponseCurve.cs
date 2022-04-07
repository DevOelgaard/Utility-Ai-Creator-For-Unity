using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

// https://forum.unity.com/threads/draw-a-line-from-a-to-b.698618/
public class ResponseCurve: AiObjectModel
{
    private Dictionary<Parameter, IDisposable> segmentDisposables = new Dictionary<Parameter, IDisposable>();
    private float minY = 0.0f;
    private float maxY = 1.0f;
    private float minX = 0.0f;
    private float maxX = 1.0f;

    public List<ResponseFunction> ResponseFunctions = new List<ResponseFunction>();
    public List<Parameter> Segments = new List<Parameter>();

    public IObservable<bool> OnParametersChanged => onParametersChanged;
    private Subject<bool> onParametersChanged = new Subject<bool>();
    public IObservable<bool> OnFunctionsChanged => onFunctionsChanged;
    private Subject<bool> onFunctionsChanged = new Subject<bool>();

    public ResponseCurve()
    {
        var firstFunction = new LinearFunction();
        ResponseFunctions = new List<ResponseFunction>();
        AddResponseFunction(firstFunction);
    }

    //public ResponseCurve(ResponseCurve original): base(original)
    //{

    //}


    protected ResponseCurve(string name, float minY = 0.0f, float maxY = 1.0f)
    {
        Name = name;
        MinY = minY;
        MaxY = maxY;
        var firstFunction = new LinearFunction();

        ResponseFunctions = new List<ResponseFunction>();
        AddResponseFunction(firstFunction);
    }
    
    public void AddResponseFunction(ResponseFunction newFunction)
    {
        var previouseFunction = ResponseFunctions.LastOrDefault();

        if (previouseFunction != null) // Not First function
        {

            var previousSegment = Segments.LastOrDefault();
            var segmentValue = 0f;
            if (previousSegment != null)
            {
                segmentValue = (float)(MaxX - Convert.ToSingle(previousSegment.Value)) / 2 + Convert.ToSingle(previousSegment.Value);

            } else
            {
                segmentValue = (MaxX - MinX) / 2;
            }
            var newSegment = new Parameter(" ", segmentValue);
            Segments.Add(newSegment);
        }
        ResponseFunctions.Add(newFunction);
        onFunctionsChanged.OnNext(true);
    }

    internal void UpdateFunction(ResponseFunction oldFunction, ResponseFunction newFunction)
    {
        var oldFunctionIndex = ResponseFunctions.IndexOf(oldFunction);
        ResponseFunctions[oldFunctionIndex] = newFunction;

        onFunctionsChanged.OnNext(true);
    }

    public void RemoveResponseFunction(ResponseFunction responseFunction)
    {
        var functionIndex = ResponseFunctions.IndexOf(responseFunction);
        var removeIndex = functionIndex - 1;
        if (removeIndex < 0) // Removing first or only function
        {
            if (ResponseFunctions.Count <= 0)
            {
                throw new Exception("Can't remove the last Response function");
            }
            ResponseFunctions.Remove(responseFunction);
            RemoveSegment(Segments[0]);
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
        onFunctionsChanged.OnNext(true);
    }

    private void RemoveSegment(Parameter segmentToRemove)
    {
        Segments.Remove(segmentToRemove);
    }

    public float CalculateResponse(float x)
    {
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
                var currentMax = (float)Convert.ToSingle(validSegments[i].Value);
                normalized = Normalize(x, previousMax, currentMax);
                previousMax = currentMax;
                result = ResponseFunctions[i].CalculateResponse(normalized, result,maxY);
            }

            normalized = Normalize(x, min, max);
            result = ResponseFunctions[indexOfLastFunction].CalculateResponse(normalized, result, maxY);
        }
        result = Mathf.Clamp(result,minY,maxY);
        return result;
    }

    private float Normalize(float value, float min, float max)
    {
        var x = (value - min) / (max - min);// * ResultFactor;
        x = Mathf.Clamp(x, 0, 1);
        return x;
    }


    internal override RestoreState GetState()
    {
        return new ResponseCurveState(Name, MinY, MaxY, Segments, this);
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

    protected override AiObjectModel InternalClone()
    {
        var clone = new ResponseCurve();
        clone.Name = Name;
        clone.Description = Description;

        clone.ResponseFunctions = new List<ResponseFunction>();
        foreach (var rf in ResponseFunctions)
        {
            var rfClone = (ResponseFunction)rf.Clone();
            clone.ResponseFunctions.Add(rfClone);
        }

        clone.Segments = new List<Parameter>();
        foreach (var s in Segments)
        {
            var pClone = new Parameter(s.Name, s.Value);
            clone.Segments.Add(pClone);
        }
        clone.MinX = MinX;
        clone.MaxX = MaxX;
        clone.MinY = MinY;
        clone.MaxY = MaxY;

        return clone;
    }

    protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        persister.SaveObject(state, path + "." + Consts.FileExtension_ResponseCurve);
        foreach (var segnemt in Segments)
        {
            var subPath = path + "/" + Consts.FolderName_Segments + "/" + Segments.IndexOf(segnemt);
            segnemt.SaveToFile(subPath, persister);
        }

        foreach(var rf in ResponseFunctions)
        {
            rf.RCIndex = ResponseFunctions.IndexOf(rf);
            var subPath = path + "/" + Consts.FolderName_ResponseFunctions + "/" + ResponseFunctions.IndexOf(rf);
            rf.SaveToFile(subPath, persister);
        }
    }

    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        var state = (ResponseCurveState)s;
        Name = state.Name;
        Description = state.Description;
        MinY = state.MinY;
        MaxY = state.MaxY;
        MinX = state.MinX;
        MaxX = state.MaxX;

        Segments = new List<Parameter>();
        var parameterStates = PersistenceAPI.Instance.LoadObjectsPathWithFiltersAndSubDirectories<RestoreState>(CurrentDirectory + Consts.FolderName_Segments, typeof(Parameter));
        foreach (var p in parameterStates)
        {
            if (p.LoadedObject == null)
            {
                var parameter = new Parameter(p.ErrorMessage, p.Exception.ToString());
                Segments.Add(parameter);
            }
            else
            {
                var parameter = Parameter.Restore<Parameter>(p.LoadedObject, restoreDebug);
                Segments.Add(parameter);
            }
        }

        Segments = Segments.OrderBy(s => Convert.ToSingle(s.Value)).ToList();

        ResponseFunctions = new List<ResponseFunction>();
        var responseFunctionStates = PersistenceAPI
            .Instance
            .LoadObjectsPathWithFiltersAndSubDirectories<ResponseFunctionState>(CurrentDirectory + Consts.FolderName_ResponseFunctions, typeof(ResponseFunction));
        foreach (var rf in responseFunctionStates)
        {
            if (rf.LoadedObject == null)
            {
                var error = new Error_ResponseFunction();
                error.Parameters.Add(new Parameter(rf.ErrorMessage, rf.Exception.ToString()));
                ResponseFunctions.Add(error);
            }
            else
            {
                var func = Restore<ResponseFunction>(rf.LoadedObject, restoreDebug);
                ResponseFunctions.Add(func);
            }
        }
        ResponseFunctions = ResponseFunctions.OrderBy(rf => rf.RCIndex).ToList();
    }

    ~ResponseCurve()
    {
        foreach(var disposable in segmentDisposables)
        {
            disposable.Value.Dispose();
        }
    }
}

[Serializable]
public class ResponseCurveState: RestoreState
{
    public string Name;
    public string Description;
    public float MinY;
    public float MaxY;
    public float MinX;
    public float MaxX;

    public List<string> segments;


    public ResponseCurveState(): base()
    {
    }

    public ResponseCurveState(string name, float minY, float maxY, List<Parameter> segments, ResponseCurve responseCurveModel): base(responseCurveModel)
    {
        Name = name;
        Description = responseCurveModel.Description;
        MinY = minY;
        MaxY = maxY;
        MinX = responseCurveModel.MinX;
        MaxX = responseCurveModel.MaxX;
    }
}