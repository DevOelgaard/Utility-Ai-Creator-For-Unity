using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;

internal class ResponseFunctionComponent: VisualElement
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();

    private readonly DropdownField typeDropdown;
    private readonly VisualElement body;
    private readonly Button removeButton;

    private ResponseFunction responseFunction;

    public IObservable<ResponseFunction> OnResponseFunctionChanged => onResponseFunctionChanged;
    private readonly Subject<ResponseFunction> onResponseFunctionChanged = new Subject<ResponseFunction>();

    public IObservable<ResponseFunction> OnRemoveClicked => onRemoveClicked;
    private readonly Subject<ResponseFunction> onRemoveClicked = new Subject<ResponseFunction>();

    public IObservable<bool> OnParametersChanged => onParametersChanged;
    private readonly Subject<bool> onParametersChanged = new Subject<bool>();


    public ResponseFunctionComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);

        typeDropdown = root.Q<DropdownField>("TypeDropdown");
        body = root.Q<VisualElement>("Body");
        removeButton = root.Q<Button>("RemoveButton");

        //typeDropdown.choices = AssetDatabaseService
        //    .GetInstancesOfType<ResponseFunction>()
        //    .Select(rF => rF.Name)
        //    .ToList();
        typeDropdown.choices = AssetDatabaseService
            .GetAssignableTypes<ResponseFunction>()
            .Select(TypeToName.ResponseFunctionToName)
            .ToList();
            //.GetInstancesOfType<ResponseFunction>()
            //.Select(rF => rF.Name)
            //.ToList();

        typeDropdown.RegisterCallback<ChangeEvent<string>>(evt =>
        {
            responseFunction = AssetDatabaseService.GetInstancesOfType<ResponseFunction>()
                .First(rF => rF.Name == evt.newValue);
            onResponseFunctionChanged.OnNext(responseFunction);
        });

        removeButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            onRemoveClicked.OnNext(responseFunction);
        });
    }

    internal void UpdateUi(ResponseFunction rF, bool disableRemoveButton = false)
    {
        this.responseFunction = rF;
        typeDropdown.value = rF.Name;

        //Debug.LogWarning("This could be more effective by using a pool");
        body.Clear();
        disposables.Clear();
        foreach(var parameter in rF.Parameters)
        {
            var pC = new ParameterComponent();
            pC.UpdateUi(parameter);
            body.Add(pC);
            parameter.OnValueChange
                .Subscribe(_ => onParametersChanged.OnNext(true))
                .AddTo(disposables);
        }

        if (disableRemoveButton)
        {
            removeButton.SetEnabled(false);
            removeButton.style.flexGrow = 0;
        }
    }

    ~ResponseFunctionComponent()
    {
        disposables.Clear();
    }
}