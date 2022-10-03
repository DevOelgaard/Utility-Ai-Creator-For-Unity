using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;
using UnityEditor.UIElements;

internal class ParameterLogViewModel : LogComponent
{
    private readonly Label parameterLabel;
    private ParameterLog parameter; 
    public ParameterLogViewModel()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);

        parameterLabel = root.Q<Label>("Identifier-Label");
    }

    internal override string GetUiName()
    {
        return parameter.Name;
    }

    internal override void UpdateUi(ILogModel element)
    {
        parameter = element as ParameterLog;
        this.style.display = DisplayStyle.Flex;
        //this.style.opacity = 1;
        parameterLabel.text = parameter.Name + ": " + parameter.Value;
    }
}