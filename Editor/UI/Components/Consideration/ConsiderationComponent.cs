using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;
using UnityEditor.UIElements;
using UnityEngine;

internal class ConsiderationComponent : AiObjectComponent 
{
    private readonly CompositeDisposable minMaxSubs = new CompositeDisposable();
    private readonly CompositeDisposable rcDisposable = new CompositeDisposable();
    private readonly TemplateContainer root;
    private Consideration considerationModel;
    private ScoreComponent baseScore => ScoreComponents[0];
    private ScoreComponent normalizedScore => ScoreComponents[1];
    private readonly VisualElement parametersContainer;
    private VisualElement curveContainer;
    private readonly EnumField performanceTag;

    private readonly ParameterComponent minParamComp;
    private readonly ParameterComponent maxParamComp;
    private FloatFieldMinMax minField;
    private FloatFieldMinMax maxField;

    private readonly TabViewComponent tabView;
    private Button responseCurveTab;
    private Button parametersTab;
    private readonly LineChartButton responseCurveButton;
    private ResponseCurveWindow responseCurveWindow;

    internal ConsiderationComponent() : base()
    {
        root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        //parametersContainer = root.Q<VisualElement>("Parameters");
        parametersContainer = new VisualElement();
        curveContainer = root.Q<VisualElement>("Curve");
        //performanceTag = root.Q<EnumField>("PerformanceTag");
        performanceTag = new EnumField("Performance");
        parametersContainer.Add(performanceTag);
        Body.Clear();
        Body.Add(root);

        tabView = new TabViewComponent();
        root.Add(tabView);

        responseCurveButton = new LineChartButton();
        responseCurveButton.name = "ResponseCurveButton";
        responseCurveButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            responseCurveWindow = WindowOpener.OpenResponseCurve();
            if (considerationModel != null)
            {
                responseCurveWindow.UpdateUi(considerationModel.CurrentResponseCurve);

                rcDisposable.Clear();
                responseCurveWindow.ResponseCurveComponent.OnResponseCurveChanged
                    .Subscribe(curve =>
                    {
                        considerationModel.CurrentResponseCurve = curve;
                        responseCurveButton.UpdateUi(considerationModel.CurrentResponseCurve);
                    })
                    .AddTo(rcDisposable);
            }
        });

        //curveContainer.Add(responseCurveLCComponent);
        tabView.AddTabGroup("Parameters", parametersContainer);
        tabView.AddTabGroup("Response Curve", responseCurveButton);

        minParamComp = new ParameterComponent();
        maxParamComp = new ParameterComponent();
        parametersContainer.Add(minParamComp);
        parametersContainer.Add(maxParamComp);

    }

    protected override void UpdateInternal(AiObjectModel model)
    {
        var sw = new System.Diagnostics.Stopwatch();
        sw.Start();
        ClearSubscriptions();
        considerationModel = model as Consideration;
        if (considerationModel != null)
        {
            considerationModel.BaseScoreChanged
                .Subscribe(score => baseScore.UpdateScore(score))
                .AddTo(modelInfoChangedDisposable);

            considerationModel.NormalizedScoreChanged
                .Subscribe(score => normalizedScore.UpdateScore(score))
                .AddTo(modelInfoChangedDisposable);

            considerationModel.OnResponseCurveChanged
                .Subscribe(curve =>
                {
                    responseCurveButton.UpdateUi(curve);
                })
                .AddTo(modelInfoChangedDisposable);

            performanceTag.Init(PerformanceTag.Normal);
            performanceTag.value = considerationModel.PerformanceTag;
            performanceTag.RegisterCallback<ChangeEvent<Enum>>(evt =>
            {
                considerationModel.PerformanceTag = (PerformanceTag) evt.newValue;
            });

            SetParameters();
            responseCurveWindow?.UpdateUi(considerationModel.CurrentResponseCurve);
            responseCurveButton.UpdateUi(considerationModel.CurrentResponseCurve);
        }

        //responseCurveLCComponent.UpdateUi(considerationModel.CurrentResponseCurve);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateInternal Consideration");
    }

    private void SetParameters()
    {
        //Debug.LogWarning("This could be more effective by using a pool");
        parametersContainer.Clear();

        minParamComp.UpdateUi(considerationModel.MinFloat);
        maxParamComp.UpdateUi(considerationModel.MaxFloat);
        minField = minParamComp.field as FloatFieldMinMax;
        maxField = maxParamComp.field as FloatFieldMinMax;
        minField.Max = (float)considerationModel.MaxFloat.Value;
        maxField.Min = (float)considerationModel.MinFloat.Value;
        
        parametersContainer.Add(performanceTag);
        parametersContainer.Add(minParamComp);
        parametersContainer.Add(maxParamComp);
        
        minMaxSubs.Clear();
        considerationModel.MinFloat
            .OnValueChange
            .Subscribe(value =>
            {
                maxField.Min = (float)value;
            })
            .AddTo(minMaxSubs);

        considerationModel.MaxFloat
            .OnValueChange
            .Subscribe(value =>
            {
                minField.Max = (float)value;
            })
            .AddTo(minMaxSubs);

        foreach(var parameter in considerationModel.Parameters)
        {
            var pC = new ParameterComponent();
            pC.UpdateUi(parameter);
            parametersContainer.Add(pC);
        }
    }

    ~ConsiderationComponent()
    {
        minMaxSubs.Clear();
        rcDisposable.Clear();
    }
}
