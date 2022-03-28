using UnityEngine.UIElements;

internal abstract class RightPanelComponent<T>: VisualElement
{
    internal abstract void UpateUi(T element);
}
