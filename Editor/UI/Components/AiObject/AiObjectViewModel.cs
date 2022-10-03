using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UniRx;
using System;

internal abstract class AiObjectViewModel : VisualElement
{
    protected readonly CompositeDisposable modelInfoChangedDisposable = new CompositeDisposable();

    private readonly Label typeLabel;
    private readonly TextField nameTextField;
    private readonly TextField descriptionTextField;
    internal AiObjectModel Model;
    protected readonly VisualElement ScoreContainer;
    protected readonly VisualElement Header;
    protected readonly VisualElement Body;
    protected readonly VisualElement Footer;
    protected InfoViewModel infoViewModel;
    protected readonly Button SaveToTemplateButton;
    protected readonly HelpBox HelpBox;
    protected readonly VisualElement HelpBoxContainer;
    private readonly LabelContainerViewModel labelContainer;


    internal List<ScoreViewModel> ScoreComponents = new List<ScoreViewModel>();

    protected AiObjectViewModel ()
    {
        var root = AssetService.GetTemplateContainer(typeof(AiObjectViewModel ).FullName);
        Add(root);
        styleSheets.Add(StylesService.GetStyleSheet("AiObjectViewModel"));

        typeLabel = root.Q<Label>("Type-Label");
        nameTextField = this.Q<TextField>("Name-TextField");
        nameTextField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (Model != null)
            {
                Model.Name = evt.newValue;
            }
        });
        
        HelpBoxContainer = root.Q<VisualElement>("HelpBoxContainer");
        HelpBox = new HelpBox("", HelpBoxMessageType.Info);
        HelpBoxContainer.Add(HelpBox);

        descriptionTextField = root.Q<TextField>("Description-TextField");
        descriptionTextField.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            if (Model != null)
            {
                Model.Description = evt.newValue;
            }
        });

        ScoreContainer = this.Q<VisualElement>("Scores");
        Header = this.Q<VisualElement>("Header");
        Body = this.Q<VisualElement>("Body");
        Footer = this.Q<VisualElement>("Footer");
        SaveToTemplateButton = this.Q<Button>("SaveToTemplate-Button");
        SaveToTemplateButton.RegisterCallback<MouseUpEvent>(SaveToTemplate);
        var lc = this.Q<VisualElement>("LabelContainer");
        labelContainer = new LabelContainerViewModel();
        lc.Add(labelContainer);

        SetFooter();
    }

    private async void SaveToTemplate(MouseUpEvent evt)
    {
        var clone = await Model.CloneAsync();
        TemplateService.Instance.Add(clone);
    }

    internal void Touch()
    {
        UpdateUi(Model);
    }

    internal void UpdateUi(AiObjectModel model)
    {
        try
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            SetLabels();
            Model = model;
            typeLabel.text = Model.GetTypeDescription();
            nameTextField.value = model.Name;
            descriptionTextField.value = model.Description;
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, model.GetType() + "Update Ui Init");
            sw.Restart();
            if (string.IsNullOrEmpty(model.HelpText))
            {
                HelpBox.style.display = DisplayStyle.None;
            } else
            {
                HelpBox.style.display = DisplayStyle.Flex;
                HelpBox.text = model.HelpText;
            }
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, model.GetType() + "Update Ui HelpBox");
            sw.Restart();
            ScoreContainer.Clear();
            ScoreComponents = new List<ScoreViewModel>();
            foreach (var scoreModel in model.ScoreModels)
            {
                var scoreComponent = new ScoreViewModel(scoreModel);
                ScoreComponents.Add(scoreComponent);
                ScoreContainer.Add(scoreComponent);
                if (scoreModel.Name == "Error")
                {
                    style.backgroundColor = new StyleColor(Color.red);
                }
            }
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, model.GetType() + "Update Ui ScoreComponents");
            sw.Restart();

            modelInfoChangedDisposable.Clear();
            Model.OnInfoChanged
                .Subscribe(info => infoViewModel.DispalyInfo(info))
                .AddTo(modelInfoChangedDisposable);
            infoViewModel.DispalyInfo(Model.Info);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, model.GetType() + "Update Ui Subscribe");
            sw.Restart();
            UpdateInternal(model);
            TimerService.Instance.LogCall(sw.ElapsedMilliseconds, model.GetType() + "Update Ui Update Internal");
            sw.Restart();
        }
        catch (Exception ex)
        {
            Model.Name = "Error Updating Ui Component";
            Model.Description = ex.ToString();
            throw;
        }
    }

    protected abstract void UpdateInternal(AiObjectModel model);

    protected virtual void SetFooter()
    {
        infoViewModel = new InfoViewModel();
        Footer.Add(infoViewModel);
    }
    
    private void SetLabels()
    {
        labelContainer.ClearLabels();
        if (Model == null) return;
        if (Model.GetType().IsSubclassOf(typeof(Consideration)) || Model.GetType() == typeof(Consideration))
        {
            if (Model is not Consideration cons) return;
        
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

    ~AiObjectViewModel ()
    {
        ClearSubscriptions();
    }

    protected virtual void ClearSubscriptions()
    {
        modelInfoChangedDisposable?.Clear();
    }
}