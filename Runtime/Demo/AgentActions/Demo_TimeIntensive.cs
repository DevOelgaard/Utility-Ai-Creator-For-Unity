using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_TimeIntensive : AgentAction
{



    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    public Demo_TimeIntensive() : base()
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>() { 
            new Parameter("ExecutionTime ms", 1) 
        };
    }

    public override void OnStart(IAiContext context)
    {
        TimerService.Instance.LogSequenceStart(Consts.Sequence_CalculateUtility_User,"FreezeXMs");
        FreezeXMs(context);
        TimerService.Instance.LogSequenceStop(Consts.Sequence_CalculateUtility_User,"FreezeXMs", true);
    }


    public override void OnGoing(IAiContext context)
    {
        TimerService.Instance.LogSequenceStart(Consts.Sequence_CalculateUtility_User,"FreezeXMs");
        FreezeXMs(context);
        TimerService.Instance.LogSequenceStop(Consts.Sequence_CalculateUtility_User,"FreezeXMs", true);
    }

    private void FreezeXMs(IAiContext context)
    {
        // stopwatch.Restart();
        // var time = stopwatch.ElapsedMilliseconds;
        // var extra = Convert.ToInt32(GetParameter("ExecutionTime ms").Value);
        // var end = time + extra;
        //
        // while (end > stopwatch.ElapsedMilliseconds)
        // {
        // }
        
        var endTime = Convert.ToInt32(GetParameter("ExecutionTime ms").Value);
        stopwatch.Restart();
        while (endTime > stopwatch.ElapsedMilliseconds)
        {
        }
        stopwatch.Stop();
    }
}
