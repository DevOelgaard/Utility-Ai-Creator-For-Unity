using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UIElements;

internal class ResponseFunctionLog: ILogModel
{
    public string Type = "";
    public List<ParameterLog> Parameters;

    internal static ResponseFunctionLog GetDebug(ResponseFunction rF, int tick)
    {
        var result = new ResponseFunctionLog();
        result.Type = rF.GetType().ToString();
        result.Parameters = new List<ParameterLog>();
        foreach(var p in rF.Parameters)
        {
            result.Parameters.Add(ParameterLog.GetDebug(p, tick));
        }
        return result;
    }
}