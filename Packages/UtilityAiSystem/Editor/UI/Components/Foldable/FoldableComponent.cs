using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class FoldableComponent : VisualElement
{
    internal AiObjectViewModel Expanded { get; private set; }
    internal  MainWindowFoldedViewModel Folded { get; private set; }

    public bool IsFolded;
    
    public FoldableComponent()
    {

    }

    internal void UpdateUi(AiObjectViewModel expanded, MainWindowFoldedViewModel folded, bool startFolded = true)
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
        
        //TODO Only touch the active element
        Expanded.Touch();
        Folded.Touch();
    }

    public VisualElement GetActiveElement()
    {
        return IsFolded ? Folded : Expanded;
    }
}
