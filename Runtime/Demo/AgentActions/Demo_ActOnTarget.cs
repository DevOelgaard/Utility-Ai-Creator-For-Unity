using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_ActOnTarget : AgentAction
{
    public Demo_ActOnTarget() : base()
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    public override void OnStart(AiContext context)
    {
        Do(context);
    }

    public override void OnGoing(AiContext context)
    {
        Do(context);
    }

    private void Do(AiContext context)
    {
        var target = GetTarget(context);
        Debug.Log("Target: " + target.name);
    }


    private GameObject GetTarget(AiContext context)
    {
        return context.GetContext<GameObject>(AiContextKey.CurrentTargetGameObject.ToString(), context.LastSelectedDecision);
    }
}
