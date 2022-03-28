using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UniRx;
using System;

public class ListViewComponent : VisualElement
{
    private TemplateContainer root;

    private Button toggleViewButton;
    private Button removeButton;
    private Button upButton;
    private Button downButton;

    public IObservable<bool> OnRemoveClicked => onRemoveClicked;
    private Subject<bool> onRemoveClicked = new Subject<bool>();

    public IObservable<bool> OnUpClicked => onUpClicked;
    private Subject<bool> onUpClicked = new Subject<bool>();

    public IObservable<bool> OnDownClicked => onDownClicked;
    private Subject<bool> onDownClicked = new Subject<bool>();

    private VisualElement centerContainer;
    private FoldableComponent FoldableComponent;

    public ListViewComponent()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
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

    internal void UpdateUi(VisualElement expanded, VisualElement folded)
    {
        FoldableComponent.UpdateUi(expanded, folded, true);
        toggleViewButton.text = FoldableComponent.IsFolded ? Consts.Text_Button_Folded : Consts.Text_Button_Expanded;
    }
}
