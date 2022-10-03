using System;
using UnityEngine.UIElements;
using UniRx;

internal class MainWindowFoldedViewModel : VisualElement
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly Label typeLabel;
    private readonly Label nameLabel;
    private readonly Label descriptionLabel;
    private readonly VisualElement scoreContainer;
    private readonly LabelContainerViewModel labelContainer;
    private readonly VisualElement footer;
    private ScoreViewModel baseScore;
    private AiObjectModel model;
    protected InfoViewModel infoViewModel;

    internal MainWindowFoldedViewModel()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);
        root.styleSheets.Clear();
        root.styleSheets.Add(StylesService.GetStyleSheet(GetType().FullName));

        typeLabel = this.Q<Label>("Type-Label-Folded");
        nameLabel = this.Q<VisualElement>("NameIdentifier").Q<Label>("Value-Label");
        descriptionLabel = this.Q<VisualElement>("DescriptionIdentifier").Q<Label>("Value-Label");
        scoreContainer = this.Q<VisualElement>("ScoreContainer");
        footer = this.Q<VisualElement>("Footer");
        var identifierContainer = this.Q<VisualElement>("IdentifierContainer");
        infoViewModel = new InfoViewModel();
        identifierContainer.Add(infoViewModel);
        var lc = this.Q<VisualElement>("LabelContainer");
        labelContainer = new LabelContainerViewModel();
        lc.Add(labelContainer);
    }

    internal void Touch()
    {
        UpdateUi(model);
    }

    internal void UpdateUi(AiObjectModel aiObjectModel)
    {

        disposables.Clear();
        this.model = aiObjectModel;

        SetLabels();
        SetFooter();

        typeLabel.text = aiObjectModel.GetTypeDescription();
        nameLabel.text = aiObjectModel.Name;
        if (aiObjectModel?.Description?.Length > 128)
        {
            descriptionLabel.text = aiObjectModel.Description.Substring(0, 128) + "...";
        }
        else
        {
            descriptionLabel.text = aiObjectModel.Description;
        }

        aiObjectModel.OnNameChanged
            .Subscribe(aiObjectName => nameLabel.text = aiObjectName)
            .AddTo(disposables);

        aiObjectModel.OnDescriptionChanged
            .Subscribe(description => descriptionLabel.text = description)
            .AddTo(disposables);
        
        scoreContainer.Clear();
        if (aiObjectModel.GetType() == typeof(Bucket) || aiObjectModel.GetType().IsAssignableFrom(typeof(Bucket)))
        {
            var b = aiObjectModel as Bucket;
            var weightComponent = new ParameterComponent();
            weightComponent.UpdateUi(b.Weight);
            scoreContainer.Add(weightComponent);
        }

        infoViewModel.DispalyInfo(aiObjectModel.Info);
        aiObjectModel.OnInfoChanged
            .Subscribe(info => infoViewModel.DispalyInfo(info))
            .AddTo(disposables);
    }

    private void SetFooter()
    {
        footer.Clear();
        if (model.GetType().IsSubclassOf(typeof(Consideration)) || model.GetType() == typeof(Consideration))
        {
            var cons = model as Consideration;
            var minLabel = new Label()
            {
                text = cons.MinFloat.Name + ": " + cons.MinFloat.Value,
                name = "FoldedFooterLabel"
            };
            footer.Add(minLabel);
            cons.MinFloat.OnValueChange
                .Subscribe(_ => minLabel.text = cons.MinFloat.Name + ": " + cons.MinFloat.Value)
                .AddTo(disposables);

            var maxLabel = new Label()
            {
                text = cons.MaxFloat.Name + ": " + cons.MaxFloat.Value,
                name = "FoldedFooterLabel"
            };
            footer.Add(maxLabel);
            cons.MinFloat.OnValueChange
                .Subscribe(_ => maxLabel.text = cons.MaxFloat.Name + ": " + cons.MaxFloat.Value)
                .AddTo(disposables);

        }

        foreach (var parameter in model.Parameters)
        {
            var labelText = parameter.Name + ": " + parameter.GetValueAsString();
            if (labelText.Length > 64)
            {
                labelText = labelText.Substring(0, 64) + "...";
            }
            var pLabel = new Label()
            {
                text = labelText,
                name = "FoldedFooterLabel"
            };
            footer.Add(pLabel);
            parameter.OnValueChange
                .Subscribe(_ => pLabel.text = parameter.Name + ": " + parameter.GetValueAsString())
                .AddTo(disposables);
        }
    }

    private void SetLabels()
    {
        labelContainer.ClearLabels();
        if (model.GetType().IsSubclassOf(typeof(Consideration)) || model.GetType() == typeof(Consideration))
        {
            if (model is not Consideration cons) return;
        
            if (cons.IsModifier)
            {
                labelContainer.AddLabel("Modifier");
            }
            
            if (cons.IsSetter)
            {
                labelContainer.AddLabel("Setter");

            }
            if (cons.IsScorer)
            {
                labelContainer.AddLabel("Scorer");
            }
            else
            {
                labelContainer.AddLabel("Boolean");
            }
            
        }
    }

    ~MainWindowFoldedViewModel()
    {
        disposables.Clear();
    }
}