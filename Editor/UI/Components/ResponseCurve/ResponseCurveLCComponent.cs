using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;
using UnityEditor.UIElements;

internal class ResponseCurveLcComponent : VisualElement
{
    private CompositeDisposable funcitonDisposables = new CompositeDisposable();
    private CompositeDisposable responseCurveDisposables = new CompositeDisposable();
    private ResponseCurve responseCurve;
    private LineChartComponent lineChart;
    internal IObservable<ResponseCurve> OnResponseCurveChanged => onResponseCurveChanged;
    private Subject<ResponseCurve> onResponseCurveChanged = new Subject<ResponseCurve>();

    private float min => Convert.ToSingle(responseCurve.MinX);
    private float max => Convert.ToSingle(responseCurve.MaxX);
    private int steps = ConstsEditor.ResponseCurve_Steps;

    //private Label nameLabel;
    private Button foldButton;
    private VisualElement curveContainer;
    private VisualElement functionsContainer;
    private VisualElement header;
    private VisualElement footer;
    private IntegerField resolution;
    private Button addFunctionButton;
    private Button saveTemplateButton;
    private DropdownField curveDropdown;

    public ResponseCurveLcComponent()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Add(root);
        root.styleSheets.Add(StylesService.GetStyleSheet("ResponseCurve"));
        header = root.Q<VisualElement>("Header");
        foldButton = root.Q<Button>("FoldButton");
        curveContainer = root.Q<VisualElement>("CurveContainer");
        functionsContainer = root.Q<VisualElement>("FunctionsContainer");
        footer = root.Q<VisualElement>("Footer");
        addFunctionButton = root.Q<Button>("AddFunctionButton");
        saveTemplateButton = root.Q<Button>("SaveTemplateButton");
        lineChart = new LineChartComponent();
        curveContainer.Add(lineChart);

        curveDropdown = root.Q<DropdownField>("ResponseCurve-Dropdown");

        addFunctionButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            var function = AssetDatabaseService
                .GetFirstInstanceOfType<ResponseFunction>();
            responseCurve.AddResponseFunction(function);
        });
        
        saveTemplateButton.RegisterCallback<MouseUpEvent>(SaveTemplate);
    }

    private async void SaveTemplate(MouseUpEvent evt)
    {
        var clone = await responseCurve.CloneAsync();
        UasTemplateService.Instance.Add(clone);
    }

    internal void UpdateUi(ResponseCurve rCurve, bool showSelection = true)
    {
        responseCurve = rCurve;
        onResponseCurveChanged.OnNext(this.responseCurve);
        if (showSelection)
        {
            header.Add(curveDropdown);
            curveDropdown.SetValueWithoutNotify(rCurve.Name);

            InitCurveDropdown();
            curveDropdown.RegisterCallback<ChangeEvent<string>>(OnCurveDropdownValueChanged);
        }
        else
        {
            curveDropdown.SetEnabled(false);
            curveDropdown.style.flexGrow = 0;
        }

        responseCurveDisposables.Clear();
        rCurve.OnFunctionsChanged
            .Subscribe(_ =>
            {
                UpdateFunctions();
            })
            .AddTo(responseCurveDisposables);

        rCurve
            .OnParametersChanged
            .Subscribe(_ =>
            {
                ReDrawChart();
            })
            .AddTo(responseCurveDisposables);

        UpdateFunctions();
    }

    private async void OnCurveDropdownValueChanged(ChangeEvent<string> evt)
    {
        if (evt.newValue is null or null) return;
        await ChangeResponseCurve(evt.newValue);
    }

    private void UpdateFunctions()
    {
        //Debug.LogWarning("This could be more effective by using a pool");
        functionsContainer.Clear();
        funcitonDisposables.Clear();
        foreach (var function in responseCurve.ResponseFunctions)
        {
            var functionComponent = new ResponseFunctionComponent();
            functionComponent.UpdateUi(function);

            if (responseCurve.ResponseFunctions.Count <= 1)
            {
                functionComponent = new ResponseFunctionComponent();
                functionComponent.UpdateUi(function, true);
            }
            functionsContainer.Add(functionComponent);

            functionComponent
                .OnParametersChanged
                .Subscribe(_ => ReDrawChart())
                .AddTo(funcitonDisposables);

            functionComponent
                .OnRemoveClicked
                .Subscribe(f => responseCurve.RemoveResponseFunction(f))
                .AddTo(funcitonDisposables);

            functionComponent
                .OnResponseFunctionChanged
                .Subscribe(f => responseCurve.UpdateFunction(function, f))
                .AddTo(funcitonDisposables);

            var funcitionIndex = responseCurve.ResponseFunctions.IndexOf(function);
            if (responseCurve.Segments.Count > funcitionIndex)
            {
                var segmentParam = responseCurve.Segments[funcitionIndex];
                var paramComponent = new ParameterComponent();
                paramComponent.name = "Segment";
                paramComponent.UpdateUi(segmentParam);
                segmentParam
                    .OnValueChange
                    .Subscribe(_ => ReDrawChart())
                    .AddTo(funcitonDisposables);

                functionsContainer.Add(paramComponent);
            }
        }
        ReDrawChart();
    }

    private void InitCurveDropdown()
    {
        var namesFromFiles = AssetDatabaseService.GetActivateableTypes(typeof(ResponseCurve));
        curveDropdown.choices = namesFromFiles
            .Where(e => !e.Name.Contains("Mock") && !e.Name.Contains("Stub"))
            .Select(e => e.Name)
            .ToList();

        foreach(var template in UasTemplateService.Instance.ResponseCurves.Values)
        {
            curveDropdown.choices.Add(template.Name);
        }
    }

    private async Task ChangeResponseCurve(string responseCurveName)
    {
        var template = UasTemplateService
            .Instance
            .ResponseCurves
            .Values
            .FirstOrDefault(e => e.Name == responseCurveName);

        if (template != null)
        {
            var clone = await template.CloneAsync() as ResponseCurve;
            UpdateUi(clone);
        } else
        {
            UpdateUi(AssetDatabaseService.GetInstanceOfType<ResponseCurve>(responseCurveName));
        }
    }

    private void ReDrawChart()
    {
        var points = new List<Vector2>();
        var stepSize = (max - min) / steps;
        for (var i = 0; i <= steps; i++)
        {
            var x = i * stepSize + min;
            var y = responseCurve.CalculateResponse(x);
            points.Add(new Vector2(x, y));
        }

        lineChart?.DrawCurve(points, min, max);
        onResponseCurveChanged.OnNext(responseCurve);
    }

    ~ResponseCurveLcComponent(){
        funcitonDisposables.Clear();
        responseCurveDisposables.Clear();
    }
}