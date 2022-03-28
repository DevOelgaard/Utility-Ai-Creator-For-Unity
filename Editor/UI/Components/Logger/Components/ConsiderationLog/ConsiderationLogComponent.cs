using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class ConsiderationLogComponent : AiObjectLogComponent
{
    private VisualElement parametersContianer;
    private VisualElement responseCurveContainer;
    private ScoreLogComponent baseScore;
    private ScoreLogComponent normalizedScore;
    private ResponseCurveLogComponent responseCurve;
    private LogComponentPool<ParameterLogComponent> parameterPool;
    private ConsiderationLog considerationLog;
    public ConsiderationLogComponent(): base()
    {
        var root = AssetDatabaseService.GetTemplateContainer(GetType().FullName);
        Body.Add(root);

        parametersContianer = root.Q<VisualElement>("ParametersContainer");
        responseCurveContainer = root.Q<VisualElement>("ResponseCurveContainer");
        baseScore = new ScoreLogComponent("BaseScore", 0.ToString());
        ScoreContainer.Add(baseScore);
        normalizedScore = new ScoreLogComponent("Normalized", 0.ToString());
        ScoreContainer.Add(normalizedScore);

        responseCurve = new ResponseCurveLogComponent();
        responseCurveContainer.Add(responseCurve);

        parameterPool = new LogComponentPool<ParameterLogComponent>(parametersContianer, false,"Parameters",1);
    }

    internal override string GetUiName()
    {
        var name = base.GetUiName() + " S: " + considerationLog.NormalizedScore.ToString("0.00");
        if (IsSelected)
        {
            name += " *S*";
        }
        else if (!IsEvaluated)
        {
            name += " *!E*";
        }
        return name;
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
