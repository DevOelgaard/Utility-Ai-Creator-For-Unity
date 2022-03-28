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

[CustomEditor(typeof(AgentMono)), CanEditMultipleObjects]
internal class AgentMonoInspector: Editor
{
    private CompositeDisposable disposables = new CompositeDisposable();
    private DropdownField aiField;
    private VisualElement root;
    //private AgentMono agent;
    private List<AgentMono> agents;

    public override VisualElement CreateInspectorGUI()
    {
        root = new VisualElement();

        var defaultInspector = new IMGUIContainer();
        defaultInspector.onGUIHandler = () => DrawDefaultInspector();
        root.Add(defaultInspector);

        aiField = new DropdownField("Default Ai");
        root.Add(aiField);

        //agent = (AgentMono)target;
        agents = targets.Cast<AgentMono>().ToList();
        //serializedObject.Update();

        SetAiFieldChoices(UASTemplateService.Instance.AIs.Values);
        UASTemplateService.Instance.AIs
            .OnValueChanged
            .Subscribe(values => SetAiFieldChoices(values))
            .AddTo(disposables);

        UASTemplateService.Instance.AIs.Values
            .ForEach(ai =>
            {
                var aiCast = ai as Ai;
                aiCast.OnIsPlayableChanged
                .Subscribe(isPlayable =>
                {
                    if (isPlayable)
                    {
                        aiField.choices.Add(ai.Name);
                    }
                    else
                    {
                        aiField.choices.Remove(ai.Name);
                    }
                })
                .AddTo(disposables);
            });

        aiField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            foreach(var agent in agents)
            {
                agent.DefaultAiName = evt.newValue;
                EditorUtility.SetDirty(agent);
            }
        });

        return root;
    }

    private void OnDisable()
    {
        disposables.Clear();
    }

    private void SetAiFieldChoices(List<AiObjectModel> ais)
    {
        aiField.choices.Clear();
        var playableAis = ais
            .Cast<Ai>()
            .Where(ai => ai.IsPLayable);

        foreach (Ai ai in playableAis)
        {
            aiField.choices.Add(ai.Name);
        }

        var agent = agents.FirstOrDefault();

        var currentAiName = playableAis.FirstOrDefault(c => c.Name == agent.DefaultAiName)?.Name;
        if (string.IsNullOrEmpty(currentAiName))
        {
            currentAiName = playableAis.FirstOrDefault()?.Name;
        }
        aiField.SetValueWithoutNotify(currentAiName);
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