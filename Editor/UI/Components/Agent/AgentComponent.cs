using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;
using System.Linq;
using MoreLinq;
using UniRx;
using UnityEditor.UIElements;
using UnityEngine;

internal class AgentComponent: RightPanelComponent<IAgent>
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly Label agentName;
    private readonly DropdownField aiDropdown;
    private readonly AiComponent aiComponent;
    private IAgent agent;

    internal AgentComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        styleSheets.Add(StylesService.GetStyleSheet("AiObjectComponent"));
        var body = root.Q<VisualElement>("Body");
        agentName = root.Q<Label>("AgentName");
        aiDropdown = root.Q<DropdownField>("Ai-Dropdown");
        var footer = root.Q<VisualElement>("Footer");
        aiComponent = new AiComponent();
        body.Add(aiComponent);

        aiDropdown.label = "AIs";
        var tickAgent = new Button();
        tickAgent.text = "TEST-Tick-Agent";
        tickAgent.RegisterCallback<MouseUpEvent>(evt =>
        {
            AiTicker.Instance.TickAgent(agent);
        });
        footer.Add(tickAgent);

        var tickAllButton = new Button();
        tickAllButton.text = "TEST-Tick-All";
        tickAllButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            AiTicker.Instance.TickAis();
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

        PlayAbleAiService.OnAisChanged
            .Subscribe(_ =>
            {
                InitDropdown();
            })
            .AddTo(disposables);

        aiDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (agent.Ai.Name == evt.newValue) return;
            agent.Ai = PlayAbleAiService.Instance.GetAiByName(evt.newValue);
            UpdateAiComponent();
        });
    }

    private async void SetAgentAiAsCurrentAgentsAi(IAgent a)
    {
        if (a == agent) return;
        var aiClone = await agent.Ai.CloneAsync();
        a.SetAi(aiClone as Ai);
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
        aiDropdown.choices = PlayAbleAiService.GetAis()
            .Select(x => x.Name)
            .ToList();

        if (agent.Ai != null && aiDropdown.choices.Contains(agent.Ai.Name))
        {
            aiDropdown.SetValueWithoutNotify(agent.Ai.Name);
        }
    }

    internal void UpdateAiComponent()
    {
        aiComponent.UpdateUi(agent.Ai);
    }

    ~AgentComponent()
    {
        disposables.Clear();
    }

}
