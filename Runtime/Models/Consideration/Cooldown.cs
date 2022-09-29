using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UniRx;

internal class Cooldown : Consideration
{
    private float cooldownTime = 0f;
    private readonly IDisposable paramaterDisposable;
    public Cooldown(): base()
    {
        ParameterContainer.AddParameter("Cooldown Time MS", 1000f);
        var cooldownTimeParam = ParameterContainer.GetParamFloat("Cooldown Time MS");
        cooldownTime = cooldownTimeParam.Value / 1000f;
        paramaterDisposable = cooldownTimeParam
            .OnValueChange
            .Subscribe(_ => 
                cooldownTime = cooldownTimeParam.Value / 1000f);
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        if(Time.time - context.CurrentEvaluatedDecision.LastSelectedTickMetaData.TickTime < cooldownTime)
        {
            return 0;
        } else
        {
            return 1;
        }
    }

    ~Cooldown()
    {
        paramaterDisposable?.Dispose();
    }
}
