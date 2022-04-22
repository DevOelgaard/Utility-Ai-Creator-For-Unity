using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class TickerModeUnrestricted : TickerMode
{
    public int TotalTicks = 0;
    public long SampleTime => (int)ParameterContainer.GetParameter("Sample Time").Value;
    private bool isStarted = false;
    private bool isLogged = false;
    private readonly Stopwatch sampleTimeSW = new Stopwatch();
    private readonly Stopwatch executionTimeSW = new Stopwatch();
    
    public TickerModeUnrestricted() : base(AiTickerMode.Unrestricted, 
        Consts.Description_TickerModeUnrestricted)
    {
        

    }

    internal override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Sample Time", 300),
            new Parameter("Run", false)
        };
    }

    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        if ((bool) ParameterContainer.GetParameter("Run").Value != true) return;
        
        if (!isStarted)
        {
            sampleTimeSW.Start();
            isStarted = true;
        }
        executionTimeSW.Start();
        agents.ForEach(agent =>
        {
            TotalTicks++;
            agent.Tick(metaData);
        });
        executionTimeSW.Stop();
        if (sampleTimeSW.ElapsedMilliseconds > SampleTime*1000)
        {
            if (!isLogged)
            {
                DebugService.Log("Sample Ended SampleTime: " + sampleTimeSW.ElapsedMilliseconds + "ms. Total ExecutionTime: " + executionTimeSW.ElapsedMilliseconds+"ms. Total Ticks: " + TotalTicks + " #Agents: " + agents.Count, this);
                isLogged = true;
            }
        }
    }
}