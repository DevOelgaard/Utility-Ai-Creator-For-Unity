using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class TickerModeTimeBudget : TickerMode
{
    private readonly System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    private int lastTickIndex = -1;
    internal int TickedAgentsThisFrame { get; private set; }
    public TickerModeTimeBudget() : base(UaiTickerMode.TimeBudget, Consts.Description_TickerModeTimeBudget)
    {
        ParameterContainer.AddParameter("Time Budget MS", 23);
        ParameterContainer.AddParameter("Debug", false);
    }
    
    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        stopwatch.Reset();
        stopwatch.Start();
        TickedAgentsThisFrame = 0;

        while(TickedAgentsThisFrame < agents.Count)
        {
            if (stopwatch.ElapsedMilliseconds >= ParameterContainer.GetParamInt("Time Budget MS").Value)
            {
                if (ParameterContainer.GetParamBool("Debug").Value)
                {
                    DebugService.Log("Breaking tickedAgents: " + TickedAgentsThisFrame + " Elapsed Time: " + stopwatch.ElapsedMilliseconds + "ms", this);
                }

                if (TickedAgentsThisFrame <= 0)
                {
                    DebugService.LogWarning("No agents ticked. The time budget may be to low! Consider increasing the Time budget or the performance of the active agents!", this);
                }
                break;
            }

            lastTickIndex++;

            if (lastTickIndex >= agents.Count)
            {
                lastTickIndex = 0;
            }

            agents[lastTickIndex].ActivateNextAction(metaData);
            TickedAgentsThisFrame++;
            if (TickedAgentsThisFrame >= agents.Count)
            {
                if (ParameterContainer.GetParamBool("Debug").Value)
                {
                    DebugService.Log("All agents ticked agents.count: " + agents.Count + " Elapsed Time: " + stopwatch.ElapsedMilliseconds + "ms", this);
                }
                break;
            }
        }
        stopwatch.Stop();
    }
}