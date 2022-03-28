using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEditor;

internal class LoggerGameRunning : LoggerState
{
    private CompositeDisposable disposables = new CompositeDisposable();
    public LoggerGameRunning(TemplateContainer root, LoggerComponent debuggerComponent)
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
        TickAgentButton.SetEnabled(true);
        //RecordToggle.SetEnabled(true);
        //RecordToggle.value = true;
        ToggleStateButton.text = "Pause";
        InfoLabelLeft.text = "Game Running";
        AiTicker.Instance
            .OnTickComplete
            .Subscribe(latestTick =>
            {
                if (RecordToggle.value)
                {
                    foreach(var agent in AgentManager.Instance.Model.Agents.Values)
                    {
                        AiLoggerService.Instance.LogTick(agent, latestTick);
                    }
                }
                SetCurrentTick(latestTick);
            })
            .AddTo(disposables);

        SetCurrentTick(AiTicker.Instance.TickCount);
    }

    internal override void OnExit()
    {
        BackLeapButton.SetEnabled(true);
        BackStepButton.SetEnabled(true);
        ForwardStepButton.SetEnabled(true);
        ForwardLeapButton.SetEnabled(true);
        TickSlider.SetEnabled(true);
        disposables.Clear();
    }

    internal override void UpdateUi(IAgent agent)
    {
        base.UpdateUi(agent);
    }

    private void ClearSubscriptions()
    {
        disposables.Clear();
    }

    ~LoggerGameRunning()
    {
        ClearSubscriptions();
    }
}