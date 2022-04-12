using UnityEngine.UIElements;

internal class LabelContainerComponent: VisualElement
{
    private TemplateContainer root;
    private VisualElement labelContainer;

    internal LabelContainerComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        labelContainer = root.Q<VisualElement>("LabelContainer");
        root.styleSheets.Add(StylesService.GetStyleSheet(GetType().FullName));
    }

    internal void ClearLabels()
    {
        labelContainer.Clear();
    }

    internal void AddLabel(string text)
    {
        labelContainer.Add(new Label
        {
            name = "TypeDescriptionLabel",
            text = text
        });
    }
}