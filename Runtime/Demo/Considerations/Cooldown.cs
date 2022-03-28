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
    private IDisposable paramaterDisposable;
    public Cooldown(): base()
    {
        cooldownTime = Convert.ToSingle(Parameters[0].Value) / 1000f;
        paramaterDisposable = Parameters[0]
            .OnValueChange
            .Subscribe(_ => cooldownTime = Convert.ToSingle(Parameters[0].Value) / 1000f);
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        if(Time.time - context.CurrentEvalutedDecision.LastSelectedTickMetaData.TickTime < cooldownTime)
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
            new Parameter("Cooldowm Time MS", 1000f)
        };
    }

    ~Cooldown()
    {
        paramaterDisposable?.Dispose();
    }
}
