using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class UCSComponent : VisualElement
{
    private HelpBox descriptionHelpBox;
    private VisualElement parametersContainer;

    public UCSComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
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
        foreach(var p in utilityContainerSelector.Parameters)
        {
            var pC = new ParameterComponent();
            parametersContainer.Add(pC);
            pC.UpdateUi(p);
        }
    }

}