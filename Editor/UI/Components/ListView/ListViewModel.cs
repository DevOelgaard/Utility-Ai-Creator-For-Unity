using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;
using System;

public class ListViewModel : VisualElement
{
    private readonly TemplateContainer root;

    private readonly Button toggleViewButton;
    private readonly Button removeButton;
    private readonly Button upButton;
    private readonly Button downButton;

    public IObservable<bool> OnRemoveClicked => onRemoveClicked;
    private readonly Subject<bool> onRemoveClicked = new Subject<bool>();

    public IObservable<bool> OnUpClicked => onUpClicked;
    private readonly Subject<bool> onUpClicked = new Subject<bool>();

    public IObservable<bool> OnDownClicked => onDownClicked;
    private readonly Subject<bool> onDownClicked = new Subject<bool>();

    private readonly VisualElement centerContainer;
    private readonly FoldableComponent FoldableComponent;

    public ListViewModel()
    {
        root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);
        toggleViewButton = root.Q<Button>("ToggleView-Button");
        removeButton = root.Q<Button>("Remove-Button");
        centerContainer = root.Q<VisualElement>("Center");
        upButton = root.Q<Button>("Up-Button");
        downButton = root.Q<Button>("Down-Button");

        root.styleSheets.Add(StylesService.GetStyleSheet("ListViewComponent"));
        FoldableComponent = new FoldableComponent();
        centerContainer.Add(FoldableComponent);

        toggleViewButton.RegisterCallback<MouseUpEvent>(_ => {
            FoldableComponent.Toggle();
            toggleViewButton.text = FoldableComponent.IsFolded ? Consts.Text_Button_Folded : Consts.Text_Button_Expanded;
        });
        
        removeButton.RegisterCallback<MouseUpEvent>(_ => {
                onRemoveClicked.OnNext(true);
        });

        upButton.RegisterCallback<MouseUpEvent>(_ =>
        {
            onUpClicked.OnNext(true);
        });

        downButton.RegisterCallback<MouseUpEvent>(_ =>
        {
            onDownClicked.OnNext(true);
        });
    }

    internal void UpdateUi(AiObjectViewModel expanded, MainWindowFoldedViewModel folded)
    {
        FoldableComponent.UpdateUi(expanded, folded, true);
        toggleViewButton.text = FoldableComponent.IsFolded ? Consts.Text_Button_Folded : Consts.Text_Button_Expanded;
    }
}
