using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_AllStates : AgentAction
{
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("OnStart", "OnStart"),
            new Parameter("OnGoing", "OnGoing"),
            new Parameter("OnEnd", "OnEnd"),
        };
    }

    public override void OnStart(AiContext context)
    {
        Debug.Log(Parameters[0].Value);
    }

    public override void OnGoing(AiContext context)
    {
        Debug.Log(Parameters[1].Value);
    }

    public override void OnEnd(AiContext context)
    {
        Debug.Log(Parameters[2].Value);
    }
}