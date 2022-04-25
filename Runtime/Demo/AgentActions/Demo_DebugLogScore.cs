using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_DebugLogScore : AgentAction
{
    public Demo_DebugLogScore() : base()
    {
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
        DebugService.Log("Bucket Score: " + context.LastSelectedBucket.Score.ToString("0.00") 
                                          + " Decision Score: " + context.LastSelectedDecision.Score.ToString("0.00") ,this);
    }
}