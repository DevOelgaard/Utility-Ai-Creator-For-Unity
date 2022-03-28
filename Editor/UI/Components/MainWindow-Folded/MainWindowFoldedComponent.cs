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
    private ScoreComponent baseScore;
    private AiObjectModel model;
    protected InfoComponent InfoComponent;

    internal MainWindowFoldedComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);

        Add(root);

        typeLabel = this.Q<Label>("Type-Label-Folded");
        nameLabel = this.Q<VisualElement>("NameIdentifier").Q<Label>("Value-Label");
        descriptionLabel = this.Q<VisualElement>("DescriptionIdentifier").Q<Label>("Value-Label");
        scoreContainer = this.Q<VisualElement>("ScoreContainer");
        var identifierContaier = this.Q<VisualElement>("IdentifierContainer");
        InfoComponent = new InfoComponent();
        identifierContaier.Add(InfoComponent);
    }

    internal void UpdateUi(AiObjectModel model)
    {
        disposables.Clear();
        this.model = model;

        typeLabel.text = model.GetType().ToString();
        nameLabel.text = model.Name;
        descriptionLabel.text = model.Description;

        model.OnNameChanged
            .Subscribe(name => nameLabel.text = name)
            .AddTo(disposables);

        model.OnDescriptionChanged
            .Subscribe(description => descriptionLabel.text = description)
            .AddTo(disposables);

        scoreContainer.Clear();
        foreach(var scoreModel in model.ScoreModels)
        {
            var scoreComponent = new ScoreComponent(scoreModel);
            scoreContainer.Add(scoreComponent);
        }
        if (model.GetType() == typeof(Bucket) || model.GetType().IsAssignableFrom(typeof(Bucket)))
        {
            var b = model as Bucket;
            var weightComponent = new ParameterComponent();
            weightComponent.UpdateUi(b.Weight);
            scoreContainer.Add(weightComponent);
        }

        InfoComponent.DispalyInfo(model.Info);
        model.OnInfoChanged
            .Subscribe(info => InfoComponent.DispalyInfo(info))
            .AddTo(disposables);
    }

    ~MainWindowFoldedComponent()
    {
        disposables.Clear();
    }
}