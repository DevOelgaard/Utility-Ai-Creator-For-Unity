using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

internal class TickerModeDesiredFrameRate : TickerMode
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private int framesThisSample = 0;
    private float elapsedTime = 0f;
    private int tickedItemsLastSample = 0;
    //private int tickedItemsLastSampleStored = 0;
    private int allowedTicksPrFrame = int.MaxValue;
    private int lastTickIndex = -1;
    internal float LastFrameRate { get; private set; } = float.MaxValue;

    internal IObservable<float> OnLastFrameRateChanged => onLastFrameRateChanged;
    private readonly Subject<float> onLastFrameRateChanged = new Subject<float>();
    internal float SampelTimeInSeconds => (float)ParameterContainer.GetParameter("Sample Time Seconds").Value;
    internal float TargetFrameRate => (float)ParameterContainer.GetParameter("Target Framerate").Value;
    private int debugTickCount = 0;


    public TickerModeDesiredFrameRate() : base(AiTickerMode.DesiredFrameRate, Consts.Description_TickerModeDesiredFrameRate)
    {
    }


    internal override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Target Framerate", (int)60),
            new Parameter("Sample Time Seconds", 1f),
            new Parameter("Debug", false),
            new Parameter("DebugTickCount", (int)0)
        };
    }

    protected override string GetFileName()
    {
        return Consts.FileName_TickerModeDesiredFrameRate;
    }

    internal override void Tick(List<IAgent> agents, TickMetaData metaData)
    {
        if (elapsedTime < SampelTimeInSeconds)
        {
            elapsedTime += Time.deltaTime;
            framesThisSample++;
        }
        else
        {
            LastFrameRate = (float)framesThisSample / elapsedTime;
            onLastFrameRateChanged.OnNext(LastFrameRate);

            if (LastFrameRate < TargetFrameRate)
            {
                var oldAllowedTicks = allowedTicksPrFrame;
                var optimizeFactor = LastFrameRate / TargetFrameRate;
                var ticksThisSample = tickedItemsLastSample * optimizeFactor;
                allowedTicksPrFrame = Mathf.FloorToInt(ticksThisSample / framesThisSample);
                if ((bool)ParameterContainer.GetParameter("Debug").Value)
                {
                    DebugService.Log("LastFrameRate: " + LastFrameRate + " TargetFrameRate: " + TargetFrameRate + " optimizeFactor: " + optimizeFactor + " tickedItemsLastSample: " + tickedItemsLastSample + " allowedTicsPrFrame: " + allowedTicksPrFrame + " oldAllowedTicks: " + oldAllowedTicks, this);
                }
            }


            if ((bool)ParameterContainer.GetParameter("Debug").Value)
            {
                DebugService.Log("LastFrameRate: " + LastFrameRate + " framesThisSample: " + framesThisSample + " elapsedTime: " + elapsedTime + " SampleTime: " + SampelTimeInSeconds, this);
            }
            framesThisSample = 0;
            elapsedTime = 0f;
            tickedItemsLastSample = 0;
        }



        var tickCountThisFrame = 0;
        foreach(var agent in agents)
        {
            tickCountThisFrame++;
            if (lastTickIndex >= agents.Count-1)
            {
                if ((bool)ParameterContainer.GetParameter("Debug").Value)
                {
                    DebugService.Log("All Agents ticked Agent count: " + agents.Count + " TickCount: " + metaData.TickCount, this);
                }
                lastTickIndex = 0;
            } else
            {
                lastTickIndex++;
            }
            agents[lastTickIndex].Tick(metaData);
            tickedItemsLastSample++;

            
            if (tickCountThisFrame >= allowedTicksPrFrame)
            {
                break;
            }
        }

        debugTickCount++;
        if ((bool)ParameterContainer.GetParameter("Debug").Value && debugTickCount >= (int)ParameterContainer.GetParameter("DebugTickCount").Value)
        {
            debugTickCount = 0;
            DebugService.Log("Framerate: " + LastFrameRate + " Allowed TicksPrFrame: " + allowedTicksPrFrame + " FrameCount: " + framesThisSample, this);
        }
    }

    ~TickerModeDesiredFrameRate()
    {
        disposables.Clear();
    }
}