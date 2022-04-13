using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public abstract class ResponseFunction: AiObjectModel
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
    // public List<Parameter> Parameters;
    private bool Inverse => (bool)Parameters
        .FirstOrDefault(p => p.Name == "Inverse" && p.Value.GetType() == typeof(bool))
        .Value; 

    protected ResponseFunction()
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

    protected override AiObjectModel InternalClone()
    {
        var clone = (ResponseFunction)Activator.CreateInstance(GetType());
        clone.Parameters = new List<Parameter>();
        foreach (var s in Parameters)
        {
            var pClone = s.Clone();
            clone.Parameters.Add(pClone);
        }

        return clone;
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
        var factor = Convert.ToSingle(Max.Value) / maxY;
        result *= factor;
        return result + prevResult;
    }

    private float Normalize(float value, float min, float max)
    {
        var factor = (max - min) / max;
        var x = value * factor;// * ResultFactor;
        return x;
    }

    protected abstract float CalculateResponseInternal(float x);

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var task = Task.Factory.StartNew(() =>
        {
            var state = (ResponseFunctionState) s;
            Name = state.Name;

            var parameters =
                RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
            Parameters = RestoreAbleService.SortByName(state.Parameters, parameters);
        });
        await task;
    }



    internal override RestoreState GetState()
    {
        return new ResponseFunctionState(this);
    }

    protected override void InternalSaveToFile(string path, IPersister destructivePersister, RestoreState state)
    {
        destructivePersister.SaveObject(state, path + "." + Consts.FileExtension_ResponseFunction);
        foreach(var parameter in Parameters)
        {
            var subPath = path +"/" + Consts.FolderName_Parameters;
            parameter.SaveToFile(subPath, destructivePersister);
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