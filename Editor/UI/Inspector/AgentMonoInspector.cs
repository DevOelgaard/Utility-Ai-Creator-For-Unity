using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;
using MoreLinq;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(AgentMono), true), CanEditMultipleObjects]
internal class AgentMonoInspector: Editor
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private DropdownField defaultAiField;
    private DropdownField currentAiField;
    private VisualElement root;
    //private AgentMono agent;
    private List<AgentMono> agents;

    public override VisualElement CreateInspectorGUI()
    {
        // Debug.Log("Creating inspector");
        root = new VisualElement();
        var defaultInspector = new IMGUIContainer();
        defaultInspector.onGUIHandler = () => DrawDefaultInspector();
        root.Add(defaultInspector);

        agents = targets.Cast<AgentMono>().ToList();
        var agent = agents.FirstOrDefault();
        
        if (EditorApplication.isPlaying)
        {
            currentAiField = new DropdownField("Current Ai");
            root.Add(currentAiField);
            SetAiFieldChoices(PlayAbleAiService.Instance.PlayAbleAIs, currentAiField, agent.defaultAiName);
            PlayAbleAiService.Instance.OnAisChanged
                .Subscribe(values => SetAiFieldChoices(values, currentAiField, agent?.Uai?.Name))
                .AddTo(disposables);
            
            currentAiField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                foreach(var agent in agents)
                {
                    var ai = PlayAbleAiService.Instance.GetAiByName(evt.newValue);
                    agent.SetAi(ai);
                    EditorUtility.SetDirty(agent);
                }
            });
        }
        else
        {
            defaultAiField = new DropdownField("Default Ai");
            root.Add(defaultAiField);
            SetAiFieldChoices(PlayAbleAiService.Instance.PlayAbleAIs, defaultAiField, agent?.defaultAiName);
            PlayAbleAiService.Instance.OnAisChanged
                .Subscribe(values => SetAiFieldChoices(values, defaultAiField, agent?.defaultAiName))
                .AddTo(disposables);

            defaultAiField.RegisterCallback<ChangeEvent<string>>(evt =>
            {
                foreach(var agent in agents)
                {
                    agent.defaultAiName = evt.newValue;
                    EditorUtility.SetDirty(agent);
                }
            });
        }

        return root;
    }

    private void OnDisable()
    {
        disposables.Clear();
    }

    private void SetAiFieldChoices(List<Uai> ais, DropdownField field, string currentValue)
    {
        field.choices.Clear();
        foreach (Uai ai in ais)
        {
            field.choices.Add(ai.Name);
        }

        var agent = agents.FirstOrDefault();

        var currentAiName = ais.FirstOrDefault(c => agent != null && c.Name == currentValue)?.Name;
        if (string.IsNullOrEmpty(currentAiName))
        {
            currentAiName = ais.FirstOrDefault()?.Name;
        }
        field.SetValueWithoutNotify(currentAiName);
    }

    private void OnDestroy()
    {
        disposables.Clear();

    }

    ~AgentMonoInspector()
    {
        disposables.Clear();
    }
}