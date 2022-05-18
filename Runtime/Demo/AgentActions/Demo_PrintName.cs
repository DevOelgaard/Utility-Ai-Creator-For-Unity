using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_PrintName : AgentAction
{
    public Demo_PrintName() : base()
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
        };
    }

    public override void OnStart(IAiContext context)
    {
        PrintName(context);
    }

    public override void OnGoing(IAiContext context)
    {
        PrintName(context);
    }

    private void PrintName(IAiContext context)
    {
        Debug.Log("Agent Name: " + context.Agent.Model.Name);
    }
}