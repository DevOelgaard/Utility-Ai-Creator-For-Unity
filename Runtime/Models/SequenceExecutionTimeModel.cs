using System.Collections.Generic;
using System.Diagnostics;
using UniRx.InternalUtil;
using Debug = UnityEngine.Debug;

namespace Models
{
    public class SequenceExecutionTimeModel
    {
        internal string Name;
        private int currentIndex = 0;
        private int exectutions = 0;
        private List<Dictionary<string, float>> executionTimeByMethods = new List<Dictionary<string, float>>();

        private float totalTicks = 0;
        public float TotalTimeUs => TicksToUs(totalTicks);
        public float AverageTimeUs => TicksToUs(totalTicks)/exectutions;
        public SequenceExecutionTimeModel(string name)
        {
            Name = name;
        }
        
        public void Add(string methodName, float ticks, bool sequenceComplete = false)
        {
            if (executionTimeByMethods.Count <= currentIndex)
            {
                executionTimeByMethods.Add(new Dictionary<string, float>());
            }
            var currentSequenceDictionary = executionTimeByMethods[currentIndex];

            if (!currentSequenceDictionary.ContainsKey(methodName))
            {
                currentSequenceDictionary.Add(methodName,0);
            }

            currentSequenceDictionary[methodName] += ticks;
            totalTicks += ticks;

            if (sequenceComplete)
            {
                exectutions++;
            }
            DebugService.Log("Logging sequence: " + Name + " - Method: " + methodName + ", Time: " + TicksToMs(ticks) + ", totalTime: " + TotalTimeUs/1000 + ", averageTime: " + AverageTimeUs/1000, this);

        }

        public void Reset()
        {
            executionTimeByMethods = new List<Dictionary<string, float>>();
        }

        public void PrintSum()
        {
            var totalTicks = 0f;
            var methodSum = new Dictionary<string, float>();
            foreach (var executionTimeByMethod in executionTimeByMethods)
            {
                foreach (var entry in executionTimeByMethod)
                {
                    if (!methodSum.ContainsKey(entry.Key))
                    {
                        methodSum.Add(entry.Key,0);
                    }

                    methodSum[entry.Key] += entry.Value;
                    totalTicks += entry.Value;
                }
            }

            var totalTimeUs = TicksToUs(totalTicks);
            var averageTotalTime = totalTimeUs / exectutions;
            Debug.Log("Sequence: " + Name + " Total Time: " + $"{totalTimeUs:0,0.0}"  + "us. Number of sequences: " + exectutions);
            Debug.Log("Sequence: " + Name + " Average Time: " + $"{averageTotalTime:0,0.0}" + "us. new avg time: " + AverageTimeUs);
        }

        private float TicksToUs(float ticks)
        {
            var microsecondsPrTick = 1000f * 1000f / Stopwatch.Frequency;
            return ticks * microsecondsPrTick;
        }
        
        private float TicksToMs(float ticks)
        {
            var microsecondsPrTick = 1000f / Stopwatch.Frequency;
            return ticks * microsecondsPrTick;
        }
    }
}