using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DoNotRepeatBucket : Consideration
{
    public DoNotRepeatBucket(): base()
    {
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        return context.LastSelectedBucket == context.CurrentEvaluatedBucket ? 0 : 1;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
