using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_TimeIntensive : AgentAction
{
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>() { 
            new Parameter("ExecutionTime ms", 3) 
        };
    }

    public override void OnStart(AiContext context)
    {
        FreezeXMs(context);
    }


    public override void OnGoing(AiContext context)
    {
        FreezeXMs(context);
    }

    private void FreezeXMs(AiContext context)
    {
        stopwatch.Reset();
        stopwatch.Start();
        var time = stopwatch.ElapsedMilliseconds;
        var extra = Convert.ToInt32(Parameters[0].Value);
        var end = time + extra;

        while (end > stopwatch.ElapsedMilliseconds)
        {
        }
        stopwatch.Stop();
    }
}
