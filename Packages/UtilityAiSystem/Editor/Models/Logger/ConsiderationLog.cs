using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ConsiderationLog : AiObjectLog
{
    internal float BaseScore;
    internal float NormalizedScore;
    internal List<ParameterLog> Parameters;
    internal ResponseCurveLog ResponseCurve;

    internal static ConsiderationLog GetDebug(Consideration consideration, int tick)
    {
        var result = new ConsiderationLog();
        result = SetBasics(result, consideration, tick) as ConsiderationLog;
        result.BaseScore = consideration.BaseScore;
        result.NormalizedScore = consideration.NormalizedScore;
        result.Parameters = new List<ParameterLog>();
        foreach(var p in consideration.Parameters)
        {
            result.Parameters.Add(ParameterLog.GetDebug(p, tick));
        }

        result.ResponseCurve = ResponseCurveLog.GetDebug(consideration.CurrentResponseCurve, tick);
        return result;
    }
}
