using System;
using System.Collections.Generic;
using System.Linq;

public class ResponseFunction: AiObjectModel
{
    public int RCIndex = -1;
    private Parameter max;
    protected Parameter Max
    {
        get
        {
            if(max == null)
            {
                max = Parameters.First(p => p.Name == "Max");
            }
            return max;
        }
    }
    public List<Parameter> Parameters;
    private bool Inverse => (bool)Parameters
        .FirstOrDefault(p => p.Name == "Inverse" && p.Value.GetType() == typeof(bool))
        .Value; 

    public ResponseFunction()
    {
        Parameters = GetParameters();
        Parameters.Add(new Parameter("Inverse", false));
        Parameters.Add(new Parameter("Max", 1f));
    }

    protected ResponseFunction(string name)
    {
        Name = name;
        Parameters = new List<Parameter>();
        Parameters = GetParameters();
        Parameters.Add(new Parameter("Inverse", false ));
        Parameters.Add(new Parameter("Max", 1f));
    }

    public ResponseFunction(ResponseFunction original): base(original)
    {
        Parameters = new List<Parameter>();
        foreach (var s in original.Parameters)
        {
            var clone = new Parameter(s.Name, s.Value);
            Parameters.Add(clone);
        }
    }

    protected virtual List<Parameter> GetParameters()
    {
        return new List<Parameter>();
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
        //result = Normalize(result, prevResult, Convert.ToSingle(Max.Value));
        var factor = Convert.ToSingle(Max.Value) / maxY;
        result *= factor;
        return result + prevResult;
        //return Normalize(result,minY,maxY);
    }

    private float Normalize(float value, float min, float max)
    {
        var factor = (max - min) / max;
        var x = value * factor;// * ResultFactor;
        return x;
    }

    protected virtual float CalculateResponseInternal(float x)
    {
        return float.MinValue;
    }

    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        var state = (ResponseFunctionState)s;
        Name = state.Name;

        var parameters = RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
        Parameters = RestoreAbleService.SortByName(state.Parameters, parameters);
    }

    internal override AiObjectModel Clone()
    {
        return new ResponseFunction(this);
    }

    internal override RestoreState GetState()
    {
        return new ResponseFunctionState(this);
    }

    protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        persister.SaveObject(state, path + "." + Consts.FileExtension_ResponseFunction);
        foreach(var parameter in Parameters)
        {
            var subPath = path +"/" + Consts.FolderName_Parameters;
            parameter.SaveToFile(subPath, persister);
        }
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
        RcIndex = responseFunction.RCIndex;
        Parameters = RestoreAbleService.NamesToList(responseFunction.Parameters);

    }
}