using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_DebugLogParameter : AgentAction
{
    private bool OnlyOngoing => (bool)Parameters.First(p => p.Name == "Only OnGoing").Value;
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
             new Parameter("OnStart", "Agent output"),
             new Parameter("OnGoing", "Agent output"),
             new Parameter("OnEnd", "Agent output"),
             new Parameter("Only OnGoing", true),

        };
    }

    public override void OnStart(AiContext context)
    {
        base.OnStart(context);

        if (!OnlyOngoing)
        {
            Debug.Log("Agent: " + context.Agent.Model.Name + " First tick: " + (string)Parameters[0].Value);
        }
    }

    public override void OnGoing(AiContext context)
    {
        base.OnGoing(context);
        Debug.Log("Agent: " + context.Agent.Model.Name + " continious tick: " + (string)Parameters[1].Value);
    }

    public override void OnEnd(AiContext context)
    {
        base.OnEnd(context);
        if (!OnlyOngoing)
        {
            Debug.Log("Agent: " + context.Agent.Model.Name + " continious tick: " + (string)Parameters[2].Value);
        }
    }
}