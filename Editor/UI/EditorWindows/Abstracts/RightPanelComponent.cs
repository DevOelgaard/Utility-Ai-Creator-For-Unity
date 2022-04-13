using UnityEngine.UIElements;

internal abstract class RightPanelComponent<T>: VisualElement
{
    internal abstract void UpdateUi(T element);
}
