using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DoNotReapeatDecision : Consideration
{
    public DoNotReapeatDecision(): base()
    {
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        return context.LastSelectedDecision == context.CurrentEvalutedDecision ? 0 : 1;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
