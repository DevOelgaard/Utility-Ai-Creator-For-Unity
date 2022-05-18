using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class DoNotRepeat : ConsiderationBoolean
{
    private Type TypeNotToRepeat => ContextAddress?.Parent?.GetType();
    public DoNotRepeat(): base()
    {
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        
        if(typeof(Bucket).IsAssignableFrom(TypeNotToRepeat))
        {
            return context.LastSelectedBucket == context.CurrentEvaluatedBucket ? 0 : 1;
        }
        else if(typeof(Decision).IsAssignableFrom(TypeNotToRepeat))
        {
            return context.LastSelectedDecision == context.CurrentEvaluatedDecision ? 0 : 1;
        }
        return 1;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            // new ParameterEnum("Container", UtilityContainerTypes.Decision)
        };
    }
}
