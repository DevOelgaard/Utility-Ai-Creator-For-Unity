using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

internal class TimerService
{
    private static TimerService _instance;
    public static TimerService Instance => _instance ??= new TimerService();

    public bool IsRecordingSequence = true;

    private readonly Dictionary<string,List<long>> values = new Dictionary<string, List<long>>();

    public TimerService()
    {
    }

    internal void LogCall(long timeInMs, string key)
    {
        if (!values.ContainsKey(key))
        {
            values.Add(key, new List<long>());
            values[key].Add(0);
            values[key].Add(0);
        } else
        {
            values[key][0]++;
            values[key][1] += timeInMs;
        }
    }

    private Dictionary<string, SequenceExecutionTimeModel> sequenceModelByName =
        new Dictionary<string, SequenceExecutionTimeModel>();

    private Dictionary<string, Stopwatch> stopwatchesByName = new Dictionary<string, Stopwatch>();
    internal void LogSequenceStart(string sequenceName, string methodName)
    {
        var key = sequenceName + methodName;
        if (!stopwatchesByName.ContainsKey(key))
        {
            var sw = new Stopwatch();
            stopwatchesByName.Add(key,sw);
        }

        var stopWatch = stopwatchesByName[key];
        if (IsRecordingSequence)
        {
            stopWatch.Restart();
        }
    }

    internal void LogSequenceStop(string sequenceName, string methodName, bool complete = false, int numberOfCalls = 1)
    {
        var key = sequenceName + methodName;
        if (!stopwatchesByName.ContainsKey(key))
        {
            var sw = new Stopwatch();
            stopwatchesByName.Add(key,sw);
        }

        var stopWatch = stopwatchesByName[key];
        if (IsRecordingSequence)
        {
            stopWatch.Stop();
            LogSequence(sequenceName,methodName,stopWatch.ElapsedTicks,complete, numberOfCalls);
        }
    }
    private void LogSequence(string sequenceName, string methodName, long elapsedTicks, bool complete = false, int numberOfCalls = 1)
    {
        if (!sequenceModelByName.ContainsKey(sequenceName))
        {
            sequenceModelByName.Add(sequenceName,new SequenceExecutionTimeModel(sequenceName));
        }

        var model = sequenceModelByName[sequenceName];
        model.Add(methodName,(float)elapsedTicks/numberOfCalls,complete);
    }
    
    internal void DebugLogSequence()
    {
        foreach (var sequenceModel in sequenceModelByName)
        {
            sequenceModel.Value.PrintSum();
        }

        if (sequenceModelByName.ContainsKey(Consts.Sequence_CalculateUtility_User) &&
            sequenceModelByName.ContainsKey(Consts.Sequence_CalculateUtility_UAI))
        {
            var userTimeUs = sequenceModelByName[Consts.Sequence_CalculateUtility_User].AverageTimeUs;
            var uaiTimeUs = sequenceModelByName[Consts.Sequence_CalculateUtility_UAI].AverageTimeUs;
            var difference = uaiTimeUs - userTimeUs;
            var formattedDiff = $"{difference:0,0.0}";
            UnityEngine.Debug.Log("Difference average: " + formattedDiff+"us");
        }
    }

    internal void Reset()
    {
        foreach(var v in values)
        {
            v.Value[0] = 0;
            v.Value[1] = 0;
        }

        sequenceModelByName = new Dictionary<string, SequenceExecutionTimeModel>();
        stopwatchesByName = new Dictionary<string, Stopwatch>();
    }

    internal void DebugLogTime()
    {
        var list = new List<KeyValuePair<string, long>>();

        foreach (var v in values)
        {
            list.Add(new KeyValuePair<string, long>(v.Key, v.Value[1]));
        }
        var ordered = list.OrderByDescending(kv => kv.Value);
        var msg = "";
        foreach (var kv in ordered)
        {
            if (kv.Key.Contains("RestoreInternal Cons "))
            {
                msg = kv.Key + ": Time " + kv.Value + " Count: " + values[kv.Key][0] + " | ";
                DebugService.Log(msg, this);
            }
        }
    }


}
