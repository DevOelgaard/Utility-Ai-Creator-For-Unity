using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Demo_ConsiderationFixedValue : Consideration
{
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