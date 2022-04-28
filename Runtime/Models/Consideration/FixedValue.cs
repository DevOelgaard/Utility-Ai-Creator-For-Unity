using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class FixedValue : Consideration
{

    public FixedValue(): base()
    {
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        return Convert.ToSingle(GetParameter("Return value").Value);
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Return value", 1f)
        };
    }
}