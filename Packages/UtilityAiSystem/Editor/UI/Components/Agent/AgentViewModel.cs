using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;
using System.Linq;
using MoreLinq;
using UniRx;
using UnityEditor.UIElements;
using UnityEngine;

internal class AgentViewModel: RightPanelComponent<IAgent>
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly Label agentName;
    private readonly DropdownField aiDropdown;
    private readonly AiViewModel aiViewModel;
    private IAgent agent;

    internal AgentViewModel()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);
        styleSheets.Add(StylesService.GetStyleSheet("AiObjectComponent"));
        var body = root.Q<VisualElement>("Body");
        agentName = root.Q<Label>("AgentName");
        aiDropdown = root.Q<DropdownField>("Ai-Dropdown");
        var footer = root.Q<VisualElement>("Footer");
        aiViewModel = new AiViewModel();
        body.Add(aiViewModel);

        aiDropdown.label = "AIs";
        var tickAgent = new Button();
        tickAgent.text = "TEST-Tick-Agent";
        tickAgent.RegisterCallback<MouseUpEvent>(evt =>
        {
            UaiTicker.Instance.TickAgent(agent);
        });
        footer.Add(tickAgent);

        var tickAllButton = new Button();
        tickAllButton.text = "TEST-Tick-All";
        tickAllButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            UaiTicker.Instance.TickAis();
        });
        footer.Add(tickAllButton);

        var applyToAllButton = new Button();
        applyToAllButton.text = "Apply to all";
        applyToAllButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            AgentManager.Instance.GetAgentsByIdentifier(agent.TypeIdentifier).Values
                .ForEach(SetAgentAiAsCurrentAgentsAi);

        });
        footer.Add(applyToAllButton);

        PlayAbleAiService.Instance.OnAisChanged
            .Subscribe(_ =>
            {
                InitDropdown();
            })
            .AddTo(disposables);

        aiDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (agent.Uai.Name == evt.newValue) return;
            agent.Uai = PlayAbleAiService.Instance.GetAiByName(evt.newValue);
            UpdateAiComponent();
        });
    }

    private async void SetAgentAiAsCurrentAgentsAi(IAgent a)
    {
        if (a == agent) return;
        var aiClone = await agent.Uai.CloneAsync();
        a.SetAi(aiClone as Uai);
    }

    internal override void UpdateUi(IAgent element)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        if (element == null) return;
        this.agent = element;

        agentName.text = agent.Model.Name;
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "AgentComponent Init");
        sw.Restart();
        InitDropdown();
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "AgentComponent InitDropdown");
        sw.Restart();
        UpdateAiComponent();
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "AgentComponent UpdateAiComponent");
        sw.Restart();
    }

    private void InitDropdown()
    {
        aiDropdown.choices = PlayAbleAiService.Instance.PlayAbleAIs
            .OrderBy(a => a.Name)
            .Select(x => x.Name)
            .ToList();

        if (agent?.Uai != null && aiDropdown.choices.Contains(agent.Uai.Name))
        {
            aiDropdown.SetValueWithoutNotify(agent.Uai.Name);
        }
    }

    internal void UpdateAiComponent()
    {
        aiViewModel.UpdateUi(agent.Uai);
    }

    ~AgentViewModel()
    {
        disposables.Clear();
    }

}
