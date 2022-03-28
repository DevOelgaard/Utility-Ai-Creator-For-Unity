using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;

internal class LoggerGameStopped : LoggerState
{
    public LoggerGameStopped(TemplateContainer root, LoggerComponent debuggerComponent)
        : base(root, debuggerComponent)
    {
    }

    internal override void OnEnter(IAgent agent)
    {
        base.OnEnter(agent);
        BackLeapButton.SetEnabled(false);
        BackStepButton.SetEnabled(false);
        ForwardStepButton.SetEnabled(false);
        ForwardLeapButton.SetEnabled(false);
        TickSlider.SetEnabled(false);
        TickAgentButton.SetEnabled(false);
        InfoLabelLeft.text = "Game Stopped";
        ToggleStateButton.text = "Play";
    }

    internal override void OnExit()
    {
        base.OnExit();
        BackLeapButton.SetEnabled(true);
        BackStepButton.SetEnabled(true);
        ForwardStepButton.SetEnabled(true);
        ForwardLeapButton.SetEnabled(true);
        TickSlider.SetEnabled(true);
    }

    internal override void TickSliderChanged(int newValue)
    {
        base.TickSliderChanged(newValue);
        SetCurrentTick(newValue);
    }

    internal override void ToggleStateButtonPressed()
    {
        EditorApplication.isPlaying = true;
    }
}
