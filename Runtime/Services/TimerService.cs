using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class TimerService
{
    private static TimerService instance;
    public static TimerService Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TimerService();
            }
            return instance;
        }
    }

    private Dictionary<string,List<long>> values = new Dictionary<string, List<long>>();

    public TimerService()
    {
        Reset();
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

    internal void Reset()
    {
        foreach(var v in values)
        {
            v.Value[0] = 0;
            v.Value[1] = 0;
        }
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
                Debug.Log(msg);
            }
        }
    }
}
