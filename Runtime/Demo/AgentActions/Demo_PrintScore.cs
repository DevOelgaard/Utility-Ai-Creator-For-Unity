using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_PrintScore : AgentAction
{
    public Demo_PrintScore() : base()
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
        PrintScore(context);
    }

    public override void OnGoing(IAiContext context)
    {
        PrintScore(context);
    }

    private void PrintScore(IAiContext context)
    {
        var decision = ContextAddress.Parent as Decision;
        var bucket = decision.ContextAddress.Parent as Bucket;
        Debug.Log("Bucket Score: " + bucket.LastCalculatedUtility + " Decision score: " + decision.LastCalculatedUtility);
    }
}