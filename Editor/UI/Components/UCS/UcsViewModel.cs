using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class UcsViewModel : VisualElement
{
    private readonly HelpBox descriptionHelpBox;
    private readonly VisualElement parametersContainer;

    public UcsViewModel()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);
        var descriptionContainer = root.Q<VisualElement>("DescriptionContainer");
        descriptionHelpBox = new HelpBox("",HelpBoxMessageType.Info); 
        descriptionContainer.Add(descriptionHelpBox);
        parametersContainer = root.Q<VisualElement>("ParametersContainer");
    }

    internal void UpdateUi(UtilityContainerSelector utilityContainerSelector)
    {
        descriptionHelpBox.text = utilityContainerSelector.GetDescription();

        parametersContainer.Clear();
        foreach(var p in utilityContainerSelector.ParameterContainer.Parameters)
        {
            var pC = new ParameterComponent();
            parametersContainer.Add(pC);
            pC.UpdateUi(p);
        }
    }

}