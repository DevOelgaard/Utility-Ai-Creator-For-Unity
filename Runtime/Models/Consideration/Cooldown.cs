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
        cooldownTime = Convert.ToSingle(GetParameter("Cooldown Time MS").Value) / 1000f;
        paramaterDisposable = GetParameter("Cooldown Time MS")
            .OnValueChange
            .Subscribe(_ => 
                cooldownTime = Convert.ToSingle(GetParameter("Cooldown Time MS").Value) / 1000f);
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

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Cooldown Time MS", 1000f),
        };
    }

    ~Cooldown()
    {
        paramaterDisposable?.Dispose();
    }
}
