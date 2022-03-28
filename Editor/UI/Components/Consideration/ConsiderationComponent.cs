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
    private CompositeDisposable minMaxSubs = new CompositeDisposable();
    private TemplateContainer root;
    private Consideration considerationModel;
    private ScoreComponent baseScore => ScoreComponents[0];
    private ScoreComponent normalizedScore => ScoreComponents[1];
    private VisualElement parametersContainer;
    private VisualElement curveContainer;
    private EnumField performanceTag;

    private ParameterComponent minParamComp;
    private ParameterComponent maxParamComp;
    private FloatFieldMinMax minField;
    private FloatFieldMinMax maxField;

    private ResponseCurveLCComponent responseCurveLCComponent;

    private TabViewComponent tabView;
    private Button responseCurveTab;
    private Button parametersTab;
    private LineChartButton responseCurveButton;
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
        responseCurveButton.RegisterCallback<MouseUpEvent>(evt =>
        {
            responseCurveWindow = WindowOpener.OpenResponseCurve();
            if (considerationModel != null)
            {
                responseCurveWindow.UpdateUi(considerationModel.CurrentResponseCurve);
            }
        });

        responseCurveLCComponent = new ResponseCurveLCComponent();
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
        considerationModel.BaseScoreChanged
            .Subscribe(score => baseScore.UpdateScore(score))
            .AddTo(modelInfoChangedDisposable);

        considerationModel.NormalizedScoreChanged
            .Subscribe(score => normalizedScore.UpdateScore(score))
            .AddTo(modelInfoChangedDisposable);


        performanceTag.Init(PerformanceTag.Normal);
        performanceTag.value = considerationModel.PerformanceTag;
        performanceTag.RegisterCallback<ChangeEvent<Enum>>(evt =>
        {
            considerationModel.PerformanceTag = (PerformanceTag)evt.newValue;
        });

        SetParameters();
        responseCurveWindow?.UpdateUi(considerationModel.CurrentResponseCurve);
        responseCurveButton.UpdateUi(considerationModel.CurrentResponseCurve);
        //responseCurveLCComponent.UpdateUi(considerationModel.CurrentResponseCurve);
        TimerService.Instance.LogCall(sw.ElapsedMilliseconds, "UpdateInternal Consideration");

    }

    private void SetParameters()
    {
        //Debug.LogWarning("This could be more effective by using a pool");
        //parametersContainer.Clear();

        minParamComp.UpdateUi(considerationModel.MinFloat);
        maxParamComp.UpdateUi(considerationModel.MaxFloat);
        minField = minParamComp.field as FloatFieldMinMax;
        maxField = maxParamComp.field as FloatFieldMinMax;
        minField.Max = Convert.ToSingle(considerationModel.MaxFloat.Value);
        maxField.Min = Convert.ToSingle(considerationModel.MinFloat.Value);
        
        parametersContainer.Add(performanceTag);
        parametersContainer.Add(minParamComp);
        parametersContainer.Add(maxParamComp);
        
        minMaxSubs.Clear();
        considerationModel.MinFloat
            .OnValueChange
            .Subscribe(value =>
            {
                maxField.Min = Convert.ToSingle(value);
            })
            .AddTo(minMaxSubs);

        considerationModel.MaxFloat
            .OnValueChange
            .Subscribe(value =>
            {
                minField.Max = Convert.ToSingle(value);
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
    }
}
