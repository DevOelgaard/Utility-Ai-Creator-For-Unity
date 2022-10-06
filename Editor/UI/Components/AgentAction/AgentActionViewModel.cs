using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

internal class AgentActionViewModel : AiObjectViewModel 
{
    private readonly VisualElement parametersContainer;
    private AgentAction agentAction;
    internal AgentActionViewModel() : base()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        parametersContainer = root.Q<VisualElement>("ParametersContainer");
        styleSheets.Add(StylesService.GetStyleSheet("AgentActionViewModel"));

        Body.Clear();
        Body.Add(root);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        this.agentAction = model as AgentAction;

        SetParameters();
    }

    private void SetParameters()
    {
        parametersContainer.Clear();
        foreach (var parameter in agentAction.Parameters)
        {
            var pC = new ParameterComponent();
            pC.UpdateUi(parameter);
            parametersContainer.Add(pC);
        }
    }
}