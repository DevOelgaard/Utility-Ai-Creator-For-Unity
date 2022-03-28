using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BoolFunction : ResponseFunction
{
    private Parameter min;
    private Parameter Min
    {
        get
        {
            if(min == null)
            {
                min = Parameters.First(p => p.Name == "Min");
            }
            return min;
        }
    }

    public BoolFunction() : base(TypeToName.RF_Bool)
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("First Value", true),
            new Parameter("CutOff", 0.5f),
            new Parameter("Min", 0f)
        };
    }

    protected override float CalculateResponseInternal(float x)
    {
        if (x < Convert.ToSingle(Parameters[1].Value))
        {
            return (bool)Parameters[0].Value == true ? Convert.ToSingle(Max.Value) : Convert.ToSingle(Min.Value);
        }
        else
        {
            return (bool)Parameters[0].Value == true ? Convert.ToSingle(Min.Value) : Convert.ToSingle(Max.Value);
        }
    }

    public override float CalculateResponse(float x, float prevResult, float maxY)
    {
        if (x < Convert.ToSingle(Parameters[1].Value))
        {
            return (bool)Parameters[0].Value == true ? Convert.ToSingle(Max.Value) : Convert.ToSingle(Min.Value);
        }
        else
        {
            return (bool)Parameters[0].Value == true ? Convert.ToSingle(Min.Value) : Convert.ToSingle(Max.Value);
        }
    }
}