using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class ConsiderAllObjectsWithTag : Decision
{
    public ConsiderAllObjectsWithTag() : base()
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Tag", "Target", ParameterTypes.Tag)
        };
    }

    protected override float CalculateUtility(AiContext context)
    {
        var targets = GameObject.FindGameObjectsWithTag((string)GetParameter("Tag").Value);
        GameObject selectedTarget = null;
        var highestUtility = 0f;
        if (targets == null)
        {
            return 0f;
        } else
        {
            foreach(var target in targets)
            {
                context.SetContext(AiContextKey.CurrentTargetGameObject, target, this);
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
                context.SetContext(AiContextKey.CurrentTargetGameObject, selectedTarget, this);
                return highestUtility;
            }
        }
    }
}
