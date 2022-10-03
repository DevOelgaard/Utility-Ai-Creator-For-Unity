using System;
using System.Collections.Generic;
using ShowCases.UAI;
using UnityEditor;

public class Desperation: ConsiderationModifier
{
    public Desperation()
    {
        Description = "Increases the weight by Desperation * Factor";
        MinFloat.Value = 0f;
        MaxFloat.Value = 1f;
        AddParameter("Factor", 1f);
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);
        return archer.Desperation;
    }

    public override float CalculateScore(IAiContext context)
    {
        var score = base.CalculateScore(context);
        if (score == 0) return float.NaN;
        var factor = ParameterContainer.GetParamFloat("Factor").Value;
        return score * factor;
    }
}