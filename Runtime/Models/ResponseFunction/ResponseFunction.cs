using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

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
                max = GetParameter("Max");
            }
            return max;
        }
    }

    private bool Inverse => (bool)GetParameter("Inverse").Value;

    protected ResponseFunction()
    {
        AddParameter(new Parameter("Inverse", false));
        AddParameter(new Parameter("Max", 1f));
    }

    protected ResponseFunction(string name)
    {
        Name = name;
        AddParameter(new Parameter("Inverse", false ));
        AddParameter(new Parameter("Max", 1f));
    }
    
    protected override AiObjectModel InternalClone()
    {
        var clone = (ResponseFunction)Activator.CreateInstance(GetType());
        // foreach (var pClone in ParametersByName
        //              .Select(s => s.Value.Clone()))
        // {
        //     clone.AddParameter(pClone);
        // }

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



    internal override RestoreState GetState()
    {
        return new ResponseFunctionState(this);
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_ResponseFunction);
        await RestoreAbleService.SaveRestoreAblesToFile(Parameters,path + "/" + Consts.FolderName_Parameters, persister);
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
        Parameters = RestoreAbleService.NamesToList(responseFunction.Parameters.ToList());

    }
}