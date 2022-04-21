using UnityEngine.UIElements;

internal class LabelContainerComponent: VisualElement
{
    private readonly TemplateContainer root;
    private readonly VisualElement labelContainer;

    internal LabelContainerComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        labelContainer = root.Q<VisualElement>("InternalLabelContainer");
        root.styleSheets.Add(StylesService.GetStyleSheet(GetType().FullName));
        Add(root);
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