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
    private CompositeDisposable disposables = new CompositeDisposable();

    private DropdownField typeDropdown;
    private VisualElement body;
    private Button removeButton;

    private ResponseFunction responseFunction;

    public IObservable<ResponseFunction> OnResponseFunctionChanged => onResponseFunctionChanged;
    private Subject<ResponseFunction> onResponseFunctionChanged = new Subject<ResponseFunction>();

    public IObservable<ResponseFunction> OnRemoveClicked => onRemoveClicked;
    private Subject<ResponseFunction> onRemoveClicked = new Subject<ResponseFunction>();

    public IObservable<bool> OnParametersChanged => onParametersChanged;
    private Subject<bool> onParametersChanged = new Subject<bool>();


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
            .Select(t => TypeToName.ResponseFunctionToName(t))
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

    internal void UpdateUi(ResponseFunction responseFunction, bool disableRemoveButton = false)
    {
        this.responseFunction = responseFunction;
        typeDropdown.value = responseFunction.Name;

        //Debug.LogWarning("This could be more effective by using a pool");
        body.Clear();
        disposables.Clear();
        foreach(var parameter in responseFunction.Parameters)
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