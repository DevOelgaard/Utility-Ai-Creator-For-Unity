using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FoldableComponent : VisualElement
{
    public VisualElement Expanded { get; private set; }
    public VisualElement Folded { get; private set; }

    public bool IsFolded;
    
    public FoldableComponent()
    {

    }

    internal void UpdateUi(VisualElement expanded, VisualElement folded, bool startFolded = true)
    {
        this.Expanded = expanded;
        this.Folded = folded;
        IsFolded = startFolded;
        UpdateDisplay();
           
    }

    public void Toggle()
    {
        IsFolded = !IsFolded;
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        //Debug.LogWarning("This may be more effective by hiding and disabling instead of swapping");
        Clear();
        Add(GetActiveElement());
    }

    public VisualElement GetActiveElement()
    {
        return IsFolded ? Folded : Expanded;
    }
}
