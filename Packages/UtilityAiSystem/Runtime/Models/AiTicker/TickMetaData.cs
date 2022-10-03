using System;
using System.Collections.Generic;
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
    public float ExecutionTime = float.MinValue;
}