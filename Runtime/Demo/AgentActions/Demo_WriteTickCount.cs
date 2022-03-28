using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_WriteTickCount : AgentAction
{
    protected override List<Parameter> GetParameters()
    {
        var result = new List<Parameter>();
        var p = new Parameter("Bool", true);
        result.Add(p);
        return result;
    }

    public override void OnStart(AiContext context)
    {
        base.OnStart(context);
        var value = context.TickMetaData.TickCount;
        Debug.Log("Agent: " + context.Agent.Model.Name + " TickCount: " + value); ;
    }
}
