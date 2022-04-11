using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;

internal class MainWindowFoldedComponent : VisualElement
{
    private CompositeDisposable disposables = new CompositeDisposable();

    private Label typeLabel;
    private Label nameLabel;
    private Label descriptionLabel;
    private VisualElement scoreContainer;
    private VisualElement labelContainer;
    private VisualElement footer;
    private ScoreComponent baseScore;
    private AiObjectModel model;
    protected InfoComponent InfoComponent;

    internal MainWindowFoldedComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        root.styleSheets.Clear();
        root.styleSheets.Add(StylesService.GetStyleSheet(GetType().FullName));

        typeLabel = this.Q<Label>("Type-Label-Folded");
        nameLabel = this.Q<VisualElement>("NameIdentifier").Q<Label>("Value-Label");
        descriptionLabel = this.Q<VisualElement>("DescriptionIdentifier").Q<Label>("Value-Label");
        scoreContainer = this.Q<VisualElement>("ScoreContainer");
        labelContainer = this.Q<VisualElement>("LabelContainer");
        footer = this.Q<VisualElement>("Footer");
        var identifierContainer = this.Q<VisualElement>("IdentifierContainer");
        InfoComponent = new InfoComponent();
        identifierContainer.Add(InfoComponent);
    }

    internal void UpdateUi(AiObjectModel aiObjectModel)
    {
        disposables.Clear();
        this.model = aiObjectModel;

        SetLabels();
        SetFooter();

        typeLabel.text = aiObjectModel.GetTypeDescription();
        nameLabel.text = aiObjectModel.Name;
        descriptionLabel.text = aiObjectModel.Description;

        aiObjectModel.OnNameChanged
            .Subscribe(aiObjectName => nameLabel.text = aiObjectName)
            .AddTo(disposables);

        aiObjectModel.OnDescriptionChanged
            .Subscribe(description => descriptionLabel.text = description)
            .AddTo(disposables);

        scoreContainer.Clear();
        // foreach(var scoreModel in aiObjectModel.ScoreModels)
        // {
        //     var scoreComponent = new ScoreComponent(scoreModel);
        //     scoreContainer.Add(scoreComponent);
        // }
        if (aiObjectModel.GetType() == typeof(Bucket) || aiObjectModel.GetType().IsAssignableFrom(typeof(Bucket)))
        {
            var b = aiObjectModel as Bucket;
            var weightComponent = new ParameterComponent();
            weightComponent.UpdateUi(b.Weight);
            scoreContainer.Add(weightComponent);
        }

        InfoComponent.DispalyInfo(aiObjectModel.Info);
        aiObjectModel.OnInfoChanged
            .Subscribe(info => InfoComponent.DispalyInfo(info))
            .AddTo(disposables);
    }

    private void SetFooter()
    {
        footer.Clear();
        if (model.GetType().IsSubclassOf(typeof(Consideration)) || model.GetType() == typeof(Consideration))
        {
            var cons = model as Consideration;
            footer.Add(new Label()
            {
                text = cons.MinFloat.Name + ": " + cons.MinFloat.Value,
                name = "FoldedFooterLabel"
            });
            
            footer.Add(new Label()
            {
                text = cons.MaxFloat.Name + ": " + cons.MaxFloat.Value,
                name = "FoldedFooterLabel"
            });
        }

        foreach (var parameter in model.Parameters)
        {
            footer.Add(new Label()
            {
                text = parameter.Name + ": " + parameter.Value.ToString(),
                name = "FoldedFooterLabel"
            });
        }
    }

    private void SetLabels()
    {
        labelContainer.Clear();
        if (model.GetType().IsSubclassOf(typeof(Consideration)) || model.GetType() == typeof(Consideration))
        {
            if (model is not Consideration cons) return;
        
            if (cons.IsModifier)
            {
                labelContainer.Add(new Label
                {
                    name = "TypeDescriptionLabel",
                    text = "Modifier"
                });
            }
            
            if (cons.IsSetter)
            {
                labelContainer.Add(new Label
                {
                    name = "TypeDescriptionLabel",
                    text = "Setter"
                });
            }
            
            if (cons.IsScorer)
            {
                labelContainer.Add(new Label
                {
                    name = "TypeDescriptionLabel",
                    text = "Scorer"
                });
            }
        }
    }

    ~MainWindowFoldedComponent()
    {
        disposables.Clear();
    }
}