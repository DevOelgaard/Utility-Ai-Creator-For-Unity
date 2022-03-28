using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ResponseCurveLog: AiObjectLog
{
    public List<ResponseFunctionLog> ResponseFunctions;

    internal static ResponseCurveLog GetDebug(ResponseCurve responseCurve, int tick)
    {
        var result = new ResponseCurveLog();
        result = SetBasics(result, responseCurve, tick) as ResponseCurveLog;
        result.ResponseFunctions = new List<ResponseFunctionLog>();
        foreach(var rF in responseCurve.ResponseFunctions)
        {
            result.ResponseFunctions.Add(ResponseFunctionLog.GetDebug(rF, tick));
        }

        return result;
    }
}