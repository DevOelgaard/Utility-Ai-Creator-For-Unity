using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

internal class RandomValue : Consideration
{
    public RandomValue(): base()
    {
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        TimerService.Instance.LogSequenceStart(Consts.Sequence_CalculateUtility_User,"Random Value");
        var value = UnityEngine.Random.Range(Convert.ToSingle(MinFloat.Value), Convert.ToSingle(MaxFloat.Value));
        TimerService.Instance.LogSequenceStop(Consts.Sequence_CalculateUtility_User,"Random Value");
        return value;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
