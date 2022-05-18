using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_WriteTickCount : AgentAction
{
    public Demo_WriteTickCount(): base()
    {
    }
    
    protected override List<Parameter> GetParameters()
    {
        var result = new List<Parameter>();
        var p = new Parameter("Bool", true);
        result.Add(p);
        return result;
    }

    public override void OnStart(IAiContext context)
    {
        base.OnStart(context);
        var value = context.TickMetaData.TickCount;
        DebugService.Log("Agent: " + context.Agent.Model.Name + " TickCount: " + value, this); ;
    }
}
