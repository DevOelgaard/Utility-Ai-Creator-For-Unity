using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class FixedValueFunction : ResponseFunction
{
    public FixedValueFunction() : base(TypeToName.RF_FixedValue)
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Return", 1f),
        };
    }

    protected override float CalculateResponseInternal(float x)
    {
        return Convert.ToSingle(Parameters[0].Value);
    }

    public override float CalculateResponse(float x, float prevResult, float maxY)
    {
        return Convert.ToSingle(Parameters[0].Value);
    }
}