using System.Collections.Generic;
using UnityEngine;

internal class Demo_PrintContext : AgentAction
{
    public Demo_PrintContext() : base()
    {
        HelpText = "Prints the context set from Decision: ConsiderAllObjectsWithTag";
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
        };
    }

    public override void OnStart(AiContext context)
    {
        Print(context);
    }

    public override void OnGoing(AiContext context)
    {
        Print(context);
    }

    private void Print(AiContext context)
    {
        var value = context.GetContext<GameObject>(AiContextKey.CurrentTargetGameObject,this);
        if (value == null)
        {
            DebugService.Log("No object found", this);
        }
        else
        {
            DebugService.Log("Found object: " + value.name, this);
        }
    }
}