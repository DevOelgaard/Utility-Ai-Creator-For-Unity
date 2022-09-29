using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class FixedValueFunction : ResponseFunction
{
    public FixedValueFunction() : base(TypeToName.RF_FixedValue)
    {
        ParameterContainer.AddParameter("Return",1f);
    }

    protected override float CalculateResponseInternal(float x)
    {
        return ParameterContainer.GetParamFloat("Return").Value;
    }

    public override float CalculateResponse(float x, float prevResult, float maxY)
    {
        return ParameterContainer.GetParamFloat("Return").Value;

    }
}