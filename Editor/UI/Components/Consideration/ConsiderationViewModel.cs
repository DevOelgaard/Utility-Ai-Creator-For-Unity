﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine.UIElements;
using UniRx;
using UnityEditor.UIElements;
using UnityEngine;

internal class ConsiderationViewModel : AiObjectViewModel 
{
    private readonly CompositeDisposable minMaxSubs = new CompositeDisposable();
    private readonly CompositeDisposable rcDisposable = new CompositeDisposable();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    private readonly TemplateContainer root;
    private Consideration considerationModel;
    private ScoreViewModel baseScore => ScoreComponents[0];
    private ScoreViewModel normalizedScore => ScoreComponents[1];
    private readonly VisualElement parametersContainer;
    private VisualElement curveContainer;
    private readonly EnumField performanceTag;

    private readonly ParameterComponent minParamComp;
    private readonly ParameterComponent maxParamComp;
    private FloatFieldMinMax minField;
    private FloatFieldMinMax maxField;

    private readonly TabViewViewModel tabView;
    private Button responseCurveTab;
    private Button parametersTab;
    private readonly ResponseCurveMinimizedViewModel responseCurveMinimized;

    internal ConsiderationViewModel() : base()
    {
        root = AssetService.GetTemplateContainer(GetType().FullName);
        //parametersContainer = root.Q<VisualElement>("Parameters");
        parametersContainer = new VisualElement();
        curveContainer = root.Q<VisualElement>("Curve");
        //performanceTag = root.Q<EnumField>("PerformanceTag");
        performanceTag = new EnumField("Performance");
        parametersContainer.Add(performanceTag);
        Body.Clear();
        Body.Add(root);

        tabView = new TabViewViewModel();
        root.Add(tabView);

        responseCurveMinimized = new ResponseCurveMinimizedViewModel()
        {
            name = "ResponseCurveButton"
        };
        responseCurveMinimized.OnResponseCurveChanged
            .Subscribe(curve =>
            {
                if (considerationModel != null) 
                    considerationModel.CurrentResponseCurve = curve;
            })
            .AddTo(disposables);

        //curveContainer.Add(responseCurveLCComponent);
        tabView.AddTabGroup("Parameters", parametersContainer);
        tabView.AddTabGroup("Response Curve", responseCurveMinimized);

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
                    responseCurveMinimized.UpdateUi(curve,considerationModel.GetUiName());
                })
                .AddTo(modelInfoChangedDisposable);
            
            responseCurveMinimized.OnResponseCurveChanged
                .Subscribe(curve =>
                {
                    considerationModel.CurrentResponseCurve = curve;
                });


            performanceTag.Init(PerformanceTag.Normal);
            performanceTag.value = considerationModel.PerformanceTag;
            performanceTag.RegisterCallback<ChangeEvent<Enum>>(evt =>
            {
                considerationModel.PerformanceTag = (PerformanceTag) evt.newValue;
            });

            SetParameters();
            responseCurveMinimized.UpdateUi(considerationModel.CurrentResponseCurve, considerationModel.GetUiName());
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
                maxField.Min = considerationModel.MinFloat.Value;
            })
            .AddTo(minMaxSubs);

        considerationModel.MaxFloat
            .OnValueChange
            .Subscribe(value =>
            {
                minField.Max = considerationModel.MaxFloat.Value;
            })
            .AddTo(minMaxSubs);

        foreach(var parameter in considerationModel.Parameters)
        {
            var pC = new ParameterComponent();
            pC.UpdateUi(parameter);
            parametersContainer.Add(pC);
        }
    }

    ~ConsiderationViewModel()
    {
        minMaxSubs.Clear();
        rcDisposable.Clear();
        disposables.Clear();
    }
}
