using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DoNotReapeat : ConsiderationBoolean
{
    private UtilityContainerTypes typeNotToRepeat => (UtilityContainerTypes)Parameters[0].Value;
    public DoNotReapeat(): base()
    {
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        if(typeNotToRepeat == UtilityContainerTypes.Bucket)
        {
            return context.LastSelectedBucket == context.CurrentEvaluatedBucket ? 0 : 1;
        }
        else if(typeNotToRepeat == UtilityContainerTypes.Decision)
        {
            return context.LastSelectedDecision == context.CurrentEvalutedDecision ? 0 : 1;
        }
        return 1;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new ParameterEnum("Container", UtilityContainerTypes.Decision)
        };
    }
}
