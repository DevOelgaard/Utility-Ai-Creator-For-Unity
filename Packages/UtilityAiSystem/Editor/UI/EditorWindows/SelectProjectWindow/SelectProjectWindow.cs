using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;

internal class SelectProjectWindow: EditorWindow
{
    private VisualElement root;
    private Button CreateProjectButton;
    private Button LoadProjectButton;
    private Action onComplete;
    private ProjectSettingsService projectSettingsService => ProjectSettingsService.Instance;

    internal void CreateGUI()
    {
        root = rootVisualElement;

        var treeAsset = AssetService.GetVisualTreeAsset(GetType().FullName);
        treeAsset.CloneTree(root);

        CreateProjectButton = root.Q<Button>("CreateButton");
        LoadProjectButton = root.Q<Button>("LoadButton");

        CreateProjectButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            CeateProject();
        });

        LoadProjectButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            LoadProject();
        });
    }

    internal void SetOnComplete(Action onComplete)
    {
        this.onComplete = onComplete;
    }

    private void CeateProject()
    {
        ProjectSettingsService.Instance.CreateProject();

        onComplete.Invoke();
        Close();
    }

    private void LoadProject()
    {
        ProjectSettingsService.Instance.LoadProject();
        onComplete.Invoke();
        Close();
    }
}