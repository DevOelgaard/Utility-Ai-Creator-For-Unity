using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;

public class ScoreViewModel: VisualElement
{
    private readonly CompositeDisposable subscriptions = new CompositeDisposable();

    private readonly TemplateContainer root;
    private readonly Label scoreName;
    private readonly Label score;
    private readonly VisualElement colorContainer;
    private readonly Color defaultColor;
    private readonly bool dynamicColor;
    public ScoreViewModel(ScoreModel model, bool dynamicColor = false)
    {
        this.dynamicColor = dynamicColor;

        root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);

        scoreName = root.Q<Label>("Name");
        score = root.Q<Label>("Score");
        colorContainer = root.Q<VisualElement>("ColorContainer");

        Setname(model.Name);
        SetScore(model.Value);

        model.OnValueChanged
            .Subscribe(value => UpdateScore(value))
            .AddTo(subscriptions);

        defaultColor = new Color(0,0,0,0);
    }

    public void UpdateScore(float score, string name = null)
    {
        if (name != null)
        {
            Setname(name);
        }
        SetScore(score);
        if (dynamicColor)
        {
            SetColor(score);
        }
    }

    internal void SetColor(float percentOfMax)
    {
        percentOfMax = Mathf.Clamp(percentOfMax, 0f, 1f);
        colorContainer.style.backgroundColor = new Color(1 - percentOfMax, percentOfMax, 0, 1);
    }

    internal void SetDefualtColor()
    {
        colorContainer.style.backgroundColor = defaultColor;

    }

    private void SetScore(float score)
    {
        this.score.text = score.ToString("F2");
    }

    private void Setname(string name)
    {
        scoreName.text = name + ": ";
    }

    ~ScoreViewModel()
    {
        subscriptions.Clear();
    }
}
