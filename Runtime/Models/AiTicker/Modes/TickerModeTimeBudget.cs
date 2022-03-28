using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class TickerModeTimeBudget : TickerMode
{
    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private int lastTickIndex = -1;
    internal int TickedAgentsThisFrame { get; private set; }
    public TickerModeTimeBudget() : base(AiTickerMode.TimeBudget, Consts.Description_TickerModeTimeBudget)
    {
    }

    internal override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Time Budget MS", (int)23),
            new Parameter("Debug", false),
        };
    }

    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        stopwatch.Reset();
        stopwatch.Start();
        TickedAgentsThisFrame = 0;

        while(TickedAgentsThisFrame < agents.Count)
        {
            if (stopwatch.ElapsedMilliseconds >= Convert.ToSingle(Parameters[0].Value))
            {
                if ((bool)Parameters[1].Value)
                {
                    Debug.Log("Breaking tickedAgents: " + TickedAgentsThisFrame + " Elapsed Time: " + stopwatch.ElapsedMilliseconds + "ms");
                }

                if (TickedAgentsThisFrame <= 0)
                {
                    Debug.LogWarning("No agents ticked. The time budget may be to low! Consider increasing the Time budget or the performance of the active agents!");
                }
                break;
            }

            lastTickIndex++;

            if (lastTickIndex >= agents.Count)
            {
                lastTickIndex = 0;
            }

            agents[lastTickIndex].Tick(metaData);
            TickedAgentsThisFrame++;
            if (TickedAgentsThisFrame >= agents.Count)
            {
                if ((bool)Parameters[1].Value)
                {
                    Debug.Log("All agents ticked agents.count: " + agents.Count + " Elapsed Time: " + stopwatch.ElapsedMilliseconds + "ms");
                }
                break;
            }
        }
        stopwatch.Stop();
    }
}