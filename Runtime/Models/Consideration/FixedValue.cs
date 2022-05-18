using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class FixedValue : Consideration
{
    protected override float CalculateBaseScore(IAiContext context)
    {
        TimerService.Instance.LogSequenceStart(Consts.Sequence_CalculateUtility_User,"CalculateBaseScore");
        var value = Convert.ToSingle(GetParameter("Return value").Value);
        TimerService.Instance.LogSequenceStop(Consts.Sequence_CalculateUtility_User,"CalculateBaseScore");
        return value;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Return value", 1f)
        };
    }
}