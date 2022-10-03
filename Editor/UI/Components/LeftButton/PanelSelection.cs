using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class PanelSelection : VisualElement
{
    private readonly VisualTreeAsset uxml;
    private readonly TemplateContainer template;

    public PanelSelection()
    {
        uxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/UAS/Scripts/UtilityAi/EditorWindow/UASManager/Components/LeftButton/PanelSelection.uxml");
        template = uxml.CloneTree();
        var l = template.Q<Label>("Text");
        l.text = "TESTER";
    }

    public VisualElement Show()
    {
        return template;
    }
}
