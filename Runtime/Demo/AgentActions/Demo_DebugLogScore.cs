﻿using System;
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
        DebugService.Log("Bucket Score: " + context.LastSelectedBucket.Score.ToString("0.00") 
                                          + " Decision Score: " + context.LastSelectedDecision.Score.ToString("0.00") ,this);
    }
}