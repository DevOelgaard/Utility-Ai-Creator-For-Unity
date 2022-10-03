using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BoolFunction : ResponseFunction
{
    private ParamFloat min;
    public ParamFloat Min
    {
        get
        {
            if(min == null)
            {
                min = ParameterContainer.GetParamFloat("Min");
            }
            return min;
        }
    }

    public BoolFunction() : base(TypeToName.RF_Bool)
    {
        ParameterContainer.AddParameter("First Value", true);
        ParameterContainer.AddParameter("CutOff", 0.5f);
        ParameterContainer.AddParameter("Min", 0f);
    }

    protected override float CalculateResponseInternal(float x)
    {
        if (x < ParameterContainer.GetParamFloat("CutOff").Value)
        {
            return ParameterContainer.GetParamBool("First Value").Value == true ? Max.Value : Min.Value;
        }
        else
        {
            return ParameterContainer.GetParamBool("First Value").Value == true ? Min.Value : Max.Value;
        }
    }

    public override float CalculateResponse(float x, float prevResult, float maxY)
    {
        if (x < ParameterContainer.GetParamFloat("CutOff").Value)
        {
            return ParameterContainer.GetParamBool("First Value").Value == true ? Max.Value : Min.Value;
        }
        else
        {
            return ParameterContainer.GetParamBool("First Value").Value == true ? Min.Value : Max.Value;
        }
    }
}