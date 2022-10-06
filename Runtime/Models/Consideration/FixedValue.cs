using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class FixedValue : Consideration
{
    public FixedValue()
    {
        AddParameter("Return value", 1f);
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        TimerService.Instance.LogSequenceStart(Consts.Sequence_CalculateUtility_User,"CalculateBaseScore");
        var value = ParameterContainer.GetParamFloat("Return value").Value;
        TimerService.Instance.LogSequenceStop(Consts.Sequence_CalculateUtility_User,"CalculateBaseScore");
        return value;
    }
}