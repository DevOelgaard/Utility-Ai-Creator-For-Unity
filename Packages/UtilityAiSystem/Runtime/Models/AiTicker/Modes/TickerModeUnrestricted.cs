using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class TickerModeUnrestricted : TickerMode
{
    public int TotalTicks = 0;
    public long SampleTime => ParameterContainer.GetParamInt("Sample Time").Value;
    private bool isStarted = false;
    private bool isLogged = false;
    private readonly Stopwatch sampleTimeSW = new Stopwatch();
    private readonly Stopwatch executionTimeSW = new Stopwatch();
    
    public TickerModeUnrestricted() : base(UaiTickerMode.Unrestricted, 
        Consts.Description_TickerModeUnrestricted)
    {
        ParameterContainer.AddParameter("Sample Time", 300);
        ParameterContainer.AddParameter("Run", false);
    }

    private int startCounter = 5;
    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        if (startCounter <= 0)
        {
            TimerService.Instance.IsRecordingSequence = true;
        }
        else
        {
            startCounter--;
        }
        if (ParameterContainer.GetParamBool("Run").Value != true) return;
        TimerService.Instance.LogSequenceStart(Consts.Sequence_CalculateUtility_UAI,"TickAgent");

        if (!isStarted)
        {
            sampleTimeSW.Start();
            isStarted = true;
        }
        executionTimeSW.Start();
        agents.ForEach(agent =>
        {
            TotalTicks++;
            agent.ActivateNextAction(metaData);
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
        TimerService.Instance.LogSequenceStop(Consts.Sequence_CalculateUtility_UAI,"TickAgent",true,agents.Count);
    }
}