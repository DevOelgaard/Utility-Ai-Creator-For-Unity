﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class ConsiderationLogViewModel : AiObjectLogViewModel
{
    private readonly ScoreLogComponent baseScore;
    private readonly ScoreLogComponent normalizedScore;
    private readonly ResponseCurveLogViewModel responseCurve;
    private readonly LogComponentPool<ParameterLogViewModel> parameterPool;
    private ConsiderationLog considerationLog;
    public ConsiderationLogViewModel(): base()
    {
        var root = AssetService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        var parametersContainer = root.Q<VisualElement>("ParametersContainer");
        var responseCurveContainer = root.Q<VisualElement>("ResponseCurveContainer");
        baseScore = new ScoreLogComponent("BaseScore", 0.ToString());
        ScoreContainer.Add(baseScore);
        normalizedScore = new ScoreLogComponent("Normalized", 0.ToString());
        ScoreContainer.Add(normalizedScore);

        responseCurve = new ResponseCurveLogViewModel();
        responseCurveContainer.Add(responseCurve);

        parameterPool = new LogComponentPool<ParameterLogViewModel>(parametersContainer, false,"Parameters",1);
    }

    internal override string GetUiName()
    {
        var uiName = base.GetUiName() + " S: " + considerationLog.NormalizedScore.ToString("0.00");
        if (IsSelected)
        {
            uiName += " *S*";
        }
        else if (!IsEvaluated)
        {
            uiName += " *!E*";
        }
        return uiName;
    }

    protected override void UpdateUiInternal(AiObjectLog aiObjectDebug)
    {
        considerationLog = aiObjectDebug as ConsiderationLog;
        NameLabel.text = considerationLog.Name; // Setting it here to avoid Double type

        var logModels = new List<ILogModel>();
        foreach(var p in considerationLog.Parameters)
        {
            logModels.Add(p);
        }

        parameterPool.Display(logModels);

        baseScore.UpdateScore(considerationLog.BaseScore);
        normalizedScore.UpdateScore(considerationLog.NormalizedScore);
        responseCurve.UpdateUi(considerationLog.ResponseCurve);
    }

    internal override void Hide()
    {
        base.Hide();
        parameterPool.Hide();
    }

    internal override void ResetColor()
    {
        base.ResetColor();
    }
}
