﻿using System.Collections.Generic;
using UnityEngine;

internal class Demo_PrintContext : AgentAction
{
    public Demo_PrintContext() : base()
    {
        HelpText = "Prints the context set from Decision: ConsiderAllObjectsWithTag";
    }

    public override void OnStart(IAiContext context)
    {
        Print(context);
    }

    public override void OnGoing(IAiContext context)
    {
        Print(context);
    }

    private void Print(IAiContext context)
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