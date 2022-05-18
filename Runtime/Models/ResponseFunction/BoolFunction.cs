using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BoolFunction : ResponseFunction
{
    private Parameter min;
    public Parameter Min
    {
        get
        {
            if(min == null)
            {
                min = GetParameter("Min");
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
        if (x < Convert.ToSingle(GetParameter("CutOff").Value))
        {
            return (bool)GetParameter("First Value").Value == true ? Convert.ToSingle(Max.Value) : Convert.ToSingle(Min.Value);
        }
        else
        {
            return (bool)GetParameter("First Value").Value == true ? Convert.ToSingle(Min.Value) : Convert.ToSingle(Max.Value);
        }
    }

    public override float CalculateResponse(float x, float prevResult, float maxY)
    {
        if (x < Convert.ToSingle(GetParameter("CutOff").Value))
        {
            return (bool)GetParameter("First Value").Value == true ? Convert.ToSingle(Max.Value) : Convert.ToSingle(Min.Value);
        }
        else
        {
            return (bool)GetParameter("First Value").Value == true ? Convert.ToSingle(Min.Value) : Convert.ToSingle(Max.Value);
        }
    }
}