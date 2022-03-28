using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using System.Linq;
using UniRx;
using System;

internal abstract class AiObjectComponent : VisualElement
{
    protected CompositeDisposable modelInfoChangedDisposable = new CompositeDisposable();

    private Label typeLabel;
    private TextField nameTextField;
    private TextField descriptionTextField;
    internal AiObjectModel Model;
    protected VisualElement ScoreContainer;
    protected VisualElement Header;
    protected VisualElement Body;
    protected VisualElement Footer;
    protected InfoComponent InfoComponent;
    protected Button SaveToTemplate;
    protected HelpBox HelpBox;
    protected VisualElement HelpBoxContainer;

    internal List<ScoreComponent> ScoreComponents = new List<ScoreComponent>();

    protected AiObjectComponent ()
    {
        var root = AssetDatabaseService.GetTemplateContainer(typeof(AiObjectComponent ).FullName);
        Add(root);

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
        SaveToTemplate = this.Q<Button>("SaveToTemplate-Button");
        SaveToTemplate.RegisterCallback<MouseUpEvent>(evt =>
        {
            var clone = Model.Clone();
            UASTemplateService.Instance.Add(clone);
        });

        SetFooter();
    }

    internal void UpdateUi(AiObjectModel model)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        Model = model;
        typeLabel.text = Model.GetType().ToString();
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
        ScoreComponents = new List<ScoreComponent>();
        foreach (var scoreModel in model.ScoreModels)
        {
            var scoreComponent = new ScoreComponent(scoreModel);
            ScoreComponents.Add(scoreComponent);
            ScoreContainer.Add(scoreComponent);
        }
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, model.GetType() + "Update Ui ScoreComponents");
        sw.Restart();

        modelInfoChangedDisposable.Clear();
        Model.OnInfoChanged
            .Subscribe(info => InfoComponent.DispalyInfo(info))
            .AddTo(modelInfoChangedDisposable);
        InfoComponent.DispalyInfo(Model.Info);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, model.GetType() + "Update Ui Subscribe");
        sw.Restart();
        UpdateInternal(model);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, model.GetType() + "Update Ui Update Internal");
        sw.Restart();
    }

    protected abstract void UpdateInternal(AiObjectModel model);

    protected virtual void SetFooter()
    {
        InfoComponent = new InfoComponent();
        Footer.Add(InfoComponent);


    }

    //internal void Close()
    //{
    //    AssetDatabase.SaveAssets();
    //}

    ~AiObjectComponent ()
    {
        ClearSubscriptions();
    }

    protected virtual void ClearSubscriptions()
    {
        modelInfoChangedDisposable?.Clear();
    }
}