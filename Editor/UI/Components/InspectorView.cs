using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public InspectorView()
    {
    }
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

}