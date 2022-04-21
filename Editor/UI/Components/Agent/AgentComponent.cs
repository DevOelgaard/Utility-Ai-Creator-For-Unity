using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;
using System.Linq;
using MoreLinq;
using UnityEditor.UIElements;
using UnityEngine;

internal class AgentComponent: RightPanelComponent<IAgent>
{
    private readonly TemplateContainer root;
    private readonly VisualElement body;
    private readonly Label agentName;
    private readonly DropdownField aiDropdown;
    private readonly AiComponent aiComponent;
    private readonly VisualElement footer;
    private readonly Button tickAgent;
    private readonly Button tickAllButton;
    private readonly Button applyToAllButton;
    private IAgent agent;
    private readonly UasTemplateService uasTemplateService;

    internal AgentComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        styleSheets.Add(StylesService.GetStyleSheet("AiObjectComponent"));
        body = root.Q<VisualElement>("Body");
        agentName = root.Q<Label>("AgentName");
        aiDropdown = root.Q<DropdownField>("Ai-Dropdown");
        footer = root.Q<VisualElement>("Footer");
        aiComponent = new AiComponent();
        body.Add(aiComponent);

        uasTemplateService = UasTemplateService.Instance;
        aiDropdown.label = "AIs";
        tickAgent = new Button();
        tickAgent.text = "TEST-Tick-Agent";
        tickAgent.RegisterCallback<MouseUpEvent>(evt =>
        {
            AiTicker.Instance.TickAgent(agent);
        });
        footer.Add(tickAgent);

        tickAllButton = new Button();
        tickAllButton.text = "TEST-Tick-All";
        tickAllButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            AiTicker.Instance.TickAis();
        });
        footer.Add(tickAllButton);

        applyToAllButton = new Button();
        applyToAllButton.text = "Apply to all";
        applyToAllButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            AgentManager.Instance.GetAgentsByIdentifier(agent.TypeIdentifier).Values
                .ForEach(SetAgentAiAsCurrentAgentsAi);

        });
        footer.Add(applyToAllButton);

        aiDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (agent.Ai.Name == evt.newValue) return;
            agent.Ai = UasTemplateService.Instance.GetAiByName(evt.newValue);
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
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "AgentComponent LoadAutoSave");
        sw.Restart();
        aiDropdown.choices = uasTemplateService
            .GetCollection(Consts.Label_UAIModel)
            .Values
            .Cast<Ai>()
            .Where(ai => ai.IsPLayable)
            .Select(x => x.Name)
            .ToList();
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "AgentComponent aiDropdown.choices");
        sw.Restart();

        if (agent.Ai != null && aiDropdown.choices.Contains(agent.Ai.Name))
        {
            aiDropdown.SetValueWithoutNotify(agent.Ai.Name);
        }
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "AgentComponent SetValueWithoutNotify");
        sw.Restart();
    }

    internal void UpdateAiComponent()
    {
        aiComponent.UpdateUi(agent.Ai);
    }

}
