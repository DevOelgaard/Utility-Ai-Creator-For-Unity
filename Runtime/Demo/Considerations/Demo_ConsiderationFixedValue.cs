using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Demo_ConsiderationFixedValue : Consideration
{

    public Demo_ConsiderationFixedValue(Demo_ConsiderationFixedValue original) : base(original)
    {
    }

    public Demo_ConsiderationFixedValue(): base()
    {
    }

    internal override AiObjectModel Clone()
    {
        return new Demo_ConsiderationFixedValue(this);
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        return Convert.ToSingle(Parameters[0].Value);
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Return value", (float)0.5f)
        };
    }
}