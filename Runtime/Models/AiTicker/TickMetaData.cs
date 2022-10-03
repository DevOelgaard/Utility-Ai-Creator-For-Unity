using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class TickMetaData
{
    public int TickCount = -1;
    public string TickedBy;
    public string TickerMessage = "";
    public readonly float TickTime = Time.time;
    public float ExecutionTimeInMicroSeconds => ExecutionTimeInTicks * TicksPrMicroSecond;
    public float ExecutionTimeInTicks = float.MinValue;
    private static float TicksPrMicroSecond => (1000f*1000f)/ Stopwatch.Frequency;

}