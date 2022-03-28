using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine.UIElements;
using System.Linq;
using MoreLinq;
using UnityEditor.UIElements;

internal class AgentComponent: RightPanelComponent<IAgent>
{
    private TemplateContainer root;
    private VisualElement body;
    private Label agentName;
    private DropdownField aiDropdown;
    private AiComponent aiComponent;
    private VisualElement footer;
    private Button tickAgent;
    private Button tickAllButton;
    private Button applyToAllButton;
    private IAgent agent;
    private UASTemplateService uasTemplateService;

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

        uasTemplateService = UASTemplateService.Instance;
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
                .ForEach(a =>
                {
                    if (a != agent)
                    {
                        var aiClone = agent.Ai.Clone() as Ai;
                        a.SetAi(aiClone);
                    }
                });

        });
        footer.Add(applyToAllButton);

        aiDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (agent.Ai.Name != evt.newValue)
            {
                agent.Ai = UASTemplateService.Instance.GetAiByName(evt.newValue);
                UpdateAiComponent();
            }
        });
    }

    internal override void UpateUi(IAgent element)
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
