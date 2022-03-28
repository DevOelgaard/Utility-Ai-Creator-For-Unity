using System;
using UnityEngine;
using UnityEngine.UIElements;

internal class InfoComponent: VisualElement
{
    private TemplateContainer root;
    private Label infoText;

    internal InfoComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        infoText = root.Q<Label>("InfoText");
    }

    internal void DispalyInfo(InfoModel info)
    {
        switch (info.InfoType)
        {
            case InfoTypes.Info:
                DisplayNormal(info.Info);
                break;
            case InfoTypes.Warning:
                DisplayWarning(info.Info);
                break;
        }
    }

    private void DisplayNormal(string text)
    {
        infoText.text = text;
        infoText.style.color = Color.white;

    }

    private void DisplayWarning(string text)
    {
        infoText.text = text;
        infoText.style.color = Color.red;
    }
}