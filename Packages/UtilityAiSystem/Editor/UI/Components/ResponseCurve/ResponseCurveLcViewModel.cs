using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;
using UnityEditor.UIElements;

internal class ResponseCurveLcViewModel : VisualElement
{
    private readonly CompositeDisposable functionDisposables = new CompositeDisposable();
    private readonly CompositeDisposable responseCurveDisposables = new CompositeDisposable();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly CompositeDisposable toggleDisposable = new CompositeDisposable();
    
    private readonly Label ownerLabel;
    private ResponseCurve responseCurve;
    private readonly LineChartComponent lineChart;

    public IObservable<ResponseCurve> OnResponseCurveChanged => onResponseCurveChanged;
    private readonly Subject<ResponseCurve> onResponseCurveChanged = new Subject<ResponseCurve>();

    private float Min => (float)responseCurve.MinX;
    private float Max => (float)responseCurve.MaxX;
    private readonly int steps = ConstsEditor.ResponseCurve_Steps;

    private Button foldButton;
    private readonly VisualElement functionsContainer;
    private readonly VisualElement header;
    private VisualElement footer;
    private readonly ToolbarToggle inverseToggle;
    private IntegerField resolution;
    private readonly DropdownField curveDropDown;

    private Guid guid;

    public ResponseCurveLcViewModel()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Add(root);
        guid = Guid.NewGuid();
        root.styleSheets.Add(StylesService.GetStyleSheet("ResponseCurve"));
        header = root.Q<VisualElement>("Header");
        ownerLabel = root.Q<Label>("Name-Label");
        foldButton = root.Q<Button>("FoldButton");
        var curveContainer = root.Q<VisualElement>("CurveContainer");
        functionsContainer = root.Q<VisualElement>("FunctionsContainer");
        footer = root.Q<VisualElement>("Footer");
        inverseToggle = root.Q<ToolbarToggle>("Inverse-ToolbarToggle");
        inverseToggle.RegisterCallback<ChangeEvent<bool>>(evt =>
        {
            if (responseCurve != null)
            {
                responseCurve.IsInversed = evt.newValue;
            }
        });
        
        var addFunctionButton = root.Q<Button>("AddFunctionButton");
        var saveTemplateButton = root.Q<Button>("SaveTemplateButton");
        lineChart = new LineChartComponent();
        curveContainer.Add(lineChart);

        curveDropDown = root.Q<DropdownField>("ResponseCurve-Dropdown");
        curveDropDown.RegisterCallback<ChangeEvent<string>>(OnCurveDropdownValueChanged);
        curveDropDown.label = "";

        addFunctionButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            var function = AssetService
                .GetFirstInstanceOfType<ResponseFunction>();
            responseCurve.AddResponseFunction(function);
        });
        
        TemplateService.Instance.ResponseCurves.OnValueChanged
            .Subscribe(InitCurveDropDownChoices)
            .AddTo(disposables);

        InitCurveDropDownChoices(TemplateService.Instance.ResponseCurves.Values);
        saveTemplateButton.RegisterCallback<MouseUpEvent>(SaveTemplate);
    }

    private async void SaveTemplate(MouseUpEvent evt)
    {
        var clone = await responseCurve.CloneAsync();
        TemplateService.Instance.Add(clone);
    }

    internal void UpdateUi(ResponseCurve rCurve, bool showSelection = true, string newOwnerName = null)
    {
        responseCurve = rCurve;
        
        inverseToggle.SetValueWithoutNotify(responseCurve.IsInversed);
        
        if (newOwnerName != null)
        {
            ownerLabel.text = "Owner: " + newOwnerName;
        }
        else
        {
            ownerLabel.text = "Template";
        }
        if (showSelection)
        {
            header.Add(curveDropDown);
            curveDropDown.SetValueWithoutNotify(rCurve.Name);
        }
        else
        {
            curveDropDown.SetEnabled(false);
            curveDropDown.style.flexGrow = 0;
        }

        responseCurveDisposables.Clear();
        responseCurve.OnCurveValueChanged
            .Subscribe(_ =>
            {
                ReDrawChart();
            })
            .AddTo(responseCurveDisposables);

        responseCurve
            .OnParametersChanged
            .Subscribe(_ =>
            {
                ReDrawChart();
            })
            .AddTo(responseCurveDisposables);

        responseCurve.OnFunctionsChanged
            .Subscribe(_ => UpdateFunctionsAndRedrawChart())
            .AddTo(responseCurveDisposables);

        UpdateFunctionsAndRedrawChart();
    }

    private async void OnCurveDropdownValueChanged(ChangeEvent<string> evt)
    {
        if (evt.newValue != null && evt.newValue != Consts.LineBreakBaseTypes &&
            evt.newValue != Consts.LineBreakTemplates && evt.newValue != Consts.LineBreakDemos)
        {
            var rC =
                await AddCopyService.GetAiObjectClone(evt.newValue, TemplateService.Instance.ResponseCurves.Values);
                
            onResponseCurveChanged.OnNext(rC as ResponseCurve);
        }
        curveDropDown.SetValueWithoutNotify(null);
    }

    private void UpdateFunctionsAndRedrawChart()
    {
        //Debug.LogWarning("This could be more effective by using a pool");
        functionsContainer.Clear();
        functionDisposables.Clear();
        foreach (var function in responseCurve.ResponseFunctions)
        {
            var functionComponent = new ResponseFunctionViewModel();
            if (responseCurve.ResponseFunctions.Count <= 1)
            {
                functionComponent.UpdateUi(function, true);
            }
            else
            {
                functionComponent.UpdateUi(function);
            }
            functionsContainer.Add(functionComponent);

            functionComponent
                .OnRemoveClicked
                .Subscribe(f => responseCurve.RemoveResponseFunction(f))
                .AddTo(functionDisposables);

            functionComponent
                .OnResponseFunctionChanged
                .Subscribe(f =>
                {
                    HandleFunctionChanged(function, f);
                    functionComponent.UpdateUi(f);
                })
                .AddTo(functionDisposables);

            var functionIndex = responseCurve.ResponseFunctions.IndexOf(function);
            if (responseCurve.Segments.Count > functionIndex)
            {
                var segmentParam = responseCurve.Segments[functionIndex];
                var paramComponent = new ParameterComponent
                {
                    name = "Segment"
                };
                paramComponent.UpdateUi(segmentParam);

                functionsContainer.Add(paramComponent);
            }
        }
        ReDrawChart();
    }

    private void HandleFunctionChanged(ResponseFunction oldFunction, ResponseFunction newFunction)
    {
        responseCurve.UpdateFunction(oldFunction, newFunction);
    }

    private void InitCurveDropDownChoices(List<AiObjectModel> responseCurveTemplates)
    {
        curveDropDown.choices = AddCopyService.GetChoices(typeof(ResponseCurve), responseCurveTemplates);
        curveDropDown.SetValueWithoutNotify(null);
    }

    private void ReDrawChart()
    {
        var points = new List<Vector2>();
        var stepSize = (Max - Min) / steps;
        for (var i = 0; i <= steps; i++)
        {
            var x = i * stepSize + Min;
            var y = responseCurve.CalculateResponse(x);
            points.Add(new Vector2(x, y));
        }

        lineChart?.DrawCurve(points, Min, Max);
        onResponseCurveChanged.OnNext(responseCurve);
    }

    ~ResponseCurveLcViewModel(){
        functionDisposables.Clear();
        responseCurveDisposables.Clear();
        disposables.Clear();
        toggleDisposable.Clear();
    }
}