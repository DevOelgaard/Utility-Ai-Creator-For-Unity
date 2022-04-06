using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DoNotRepeatBucket : Consideration
{
    public DoNotRepeatBucket(DoNotRepeatBucket original) : base(original)
    {
    }

    public DoNotRepeatBucket(): base()
    {
    }

    internal override AiObjectModel Clone()
    {
        return new DoNotRepeatBucket(this);
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
