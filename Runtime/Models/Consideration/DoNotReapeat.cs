using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DoNotReapeat : ConsiderationBoolean
{
    private UtilityContainerTypes TypeNotToRepeat => (UtilityContainerTypes)GetParameter("Container").Value;
    public DoNotReapeat(): base()
    {
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        if(TypeNotToRepeat == UtilityContainerTypes.Bucket)
        {
            return context.LastSelectedBucket == context.CurrentEvaluatedBucket ? 0 : 1;
        }
        else if(TypeNotToRepeat == UtilityContainerTypes.Decision)
        {
            return context.LastSelectedDecision == context.CurrentEvaluatedDecision ? 0 : 1;
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
