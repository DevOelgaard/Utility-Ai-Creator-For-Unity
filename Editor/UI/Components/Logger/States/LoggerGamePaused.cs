using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

internal class LoggerGamePaused : LoggerState
{
    private CompositeDisposable disposables = new CompositeDisposable();
    public LoggerGamePaused(TemplateContainer root, LoggerComponent debuggerComponent) 
        : base(root, debuggerComponent)
    {
    }

    internal override void OnEnter(IAgent agent)
    {
        base.OnEnter(agent);
        TickAgentButton.SetEnabled(false);
        BackLeapButton.SetEnabled(true);
        BackStepButton.SetEnabled(true);
        ForwardStepButton.SetEnabled(true);
        ForwardLeapButton.SetEnabled(true);
        ToggleStateButton.text = "Resume";
        InfoLabelLeft.text = "Game Paused";

    }

    internal override void OnExit()
    {
        base.OnExit();
    }

    internal override void KeyPressed(KeyDownEvent key)
    {
        if(key.keyCode == KeyCode.RightArrow && key.ctrlKey)
        {
            SetCurrentTick(AiTicker.Instance.TickCount);
        }
        else if(key.keyCode == KeyCode.RightArrow)
        {
            SetCurrentTick(CurrentTick + ConstsEditor.Logger_StepSize);
        }
        else if (key.keyCode == KeyCode.LeftArrow && key.ctrlKey)
        {
            SetCurrentTick(0);

        }
        else if (key.keyCode == KeyCode.LeftArrow)
        {
            SetCurrentTick(CurrentTick - ConstsEditor.Logger_StepSize);
        }
    }

    internal override void UpdateUi(IAgent agent)
    {
        Agent = agent;
        if (agent != null)
        {
            InspectAi(CurrentTick);
        }
    }

    internal override void BackLeapButtonPressed()
    {
        SetCurrentTick(CurrentTick - ConstsEditor.Debugger_LeapSize);

    }

    internal override void BackStepButtonPressed()
    {
        SetCurrentTick(CurrentTick - ConstsEditor.Logger_StepSize);
    }

    internal override void ForwardStepButtonPressed()
    {
        SetCurrentTick(CurrentTick + ConstsEditor.Logger_StepSize);
    }

    internal override void ForwardLeapButtonPressed()
    {
        SetCurrentTick(CurrentTick + ConstsEditor.Debugger_LeapSize);
    }

    internal override void TickSliderChanged(int newValue)
    {
        base.TickSliderChanged(newValue);
        SetCurrentTick(newValue);
    }

    protected override void SetCurrentTick(int tick)
    {
        if (tick > AiTicker.Instance.TickCount)
        {
            AiTicker.Instance.TickUntilCount(tick,true);
        }
        base.SetCurrentTick(tick);
    }

    private void ClearSubscriptions()
    {
        disposables.Clear();
    }


    ~LoggerGamePaused()
    {
        ClearSubscriptions();
    }
}