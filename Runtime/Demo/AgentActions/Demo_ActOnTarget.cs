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

    public override void OnStart(IAiContext context)
    {
        Do(context);
    }

    public override void OnGoing(IAiContext context)
    {
        Do(context);
    }

    private void Do(IAiContext context)
    {
        var target = GetTarget(context);
        DebugService.Log("Target: " + target.name, this);
    }


    private GameObject GetTarget(IAiContext context)
    {
        return context.GetContext<GameObject>(AiContextKey.CurrentTargetGameObject.ToString(), context.LastSelectedDecision);
    }
}
