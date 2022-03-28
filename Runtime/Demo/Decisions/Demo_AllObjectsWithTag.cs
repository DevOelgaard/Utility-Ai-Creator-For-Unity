using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_AllObjectsWithTag : Decision
{
    public Demo_AllObjectsWithTag() : base()
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Tag", "Target", ParameterEnum.Tag)
        };
    }

    protected override float CalculateUtility(AiContext context)
    {
        var targets = GameObject.FindGameObjectsWithTag((string)Parameters[0].Value);
        GameObject selectedTarget = null;
        var highestUtility = 0f;
        var address = GetContextAddress(context);

        if (targets == null)
        {
            return 0f;
        } else
        {
            foreach(var target in targets)
            {
                context.SetContext(address+AiContextKey.CurrentTargetGameObject, target);
                var utility = context.UtilityScorer.CalculateUtility(Considerations.Values, context);
                if (utility > highestUtility)
                {
                    highestUtility = utility;
                    selectedTarget = target;
                }
            }
            if (selectedTarget == null)
            {
                return 0f;
            } else
            {
                context.SetContext(address + AiContextKey.CurrentTargetGameObject, selectedTarget);
                return highestUtility;
            }
        }
    }
}
