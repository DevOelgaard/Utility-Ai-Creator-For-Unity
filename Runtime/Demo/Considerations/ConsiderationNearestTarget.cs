using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class ConsiderationNearestTarget : Consideration
{
    public ConsiderationNearestTarget()
    {
        MinFloat.Value = 0f;
        MaxFloat.Value = 1000f;
        HelpText = "The parent must set a " + AiContextKey.CurrentTargetGameObject + " for it to evaluate";
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        var agent = (AgentMono)context.Agent;
        var address = "";
        if (context.CurrentEvalutedDecision != null)
        {
            address = context.CurrentEvalutedDecision.GetContextAddress(context);

        }
        else
        {
            address = context.CurrentEvaluatedBucket.GetContextAddress(context);
        }
        var target = context.GetContext<GameObject>(address + AiContextKey.CurrentTargetGameObject);
        if (target == null)
        {
            return 0;
        }
        var distance = Vector3.Distance(agent.transform.position, target.transform.position);
        return Convert.ToSingle(MaxFloat.Value) - distance;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
