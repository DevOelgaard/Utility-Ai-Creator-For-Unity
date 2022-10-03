using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;

internal class ResponseFunctionViewModel: VisualElement
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly CompositeDisposable responseFunctionDisposables = new CompositeDisposable();

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


    public ResponseFunctionViewModel()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);

        typeDropdown = root.Q<DropdownField>("TypeDropdown");
        body = root.Q<VisualElement>("Body");
        removeButton = root.Q<Button>("RemoveButton");

        typeDropdown.choices = AddCopyService.GetChoices(typeof(ResponseFunction));
        typeDropdown.RegisterCallback<ChangeEvent<string>>(ChangeResponseFunction);

        removeButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            onRemoveClicked.OnNext(responseFunction);
        });
    }

    private async void ChangeResponseFunction(ChangeEvent<string> evt)
    {
        if (evt.newValue != null && evt.newValue != Consts.LineBreakBaseTypes && evt.newValue != Consts.LineBreakTemplates && evt.newValue != Consts.LineBreakDemos)
        {
            var newRf = await AddCopyService.GetAiObjectClone(evt.newValue, null);
            onResponseFunctionChanged.OnNext(newRf as ResponseFunction);
        }
    }

    internal void UpdateUi(ResponseFunction rF, bool disableRemoveButton = false)
    {
        this.responseFunction = rF;
        typeDropdown.SetValueWithoutNotify(rF.Name);

        body.Clear();
        disposables.Clear();
        foreach(var parameter in responseFunction.Parameters)
        {
            var pC = new ParameterComponent();
            pC.UpdateUi(parameter);
            body.Add(pC);
            // parameter.OnValueChange
            //     .Subscribe(_ => onParametersChanged.OnNext(true))
            //     .AddTo(disposables);
        }

        if (disableRemoveButton)
        {
            removeButton.SetEnabled(false);
            removeButton.style.flexGrow = 0;
        }
    }

    ~ResponseFunctionViewModel()
    {
        disposables.Clear();
        responseFunctionDisposables.Clear();
    }
}