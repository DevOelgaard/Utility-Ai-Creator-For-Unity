using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UIElements;

internal class AgentActionComponent : AiObjectComponent 
{
    private readonly VisualElement parametersContainer;
    private AgentAction agentAction;
    internal AgentActionComponent() : base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        parametersContainer = root.Q<VisualElement>("ParametersContainer");

        Body.Clear();
        Body.Add(root);
    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        this.agentAction = model as AgentAction;
        if (model.Name == "Error")
        {
            style.backgroundColor = new StyleColor(Color.red);
        }
        SetParameters();
    }

    private void SetParameters()
    {
        parametersContainer.Clear();
        foreach (var parameter in agentAction.Parameters)
        {
            if (parameter.Name == "Error")
            {
                style.backgroundColor = new StyleColor(Color.red);
            }
            var pC = new ParameterComponent();
            pC.UpdateUi(parameter);
            parametersContainer.Add(pC);
        }
    }
}

