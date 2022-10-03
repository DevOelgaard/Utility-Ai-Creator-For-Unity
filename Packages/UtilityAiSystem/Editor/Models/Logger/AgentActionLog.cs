using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentActionLog: AiObjectLog
{
    internal List<ParameterLog> Parameters;

    internal static AgentActionLog GetDebug(AgentAction aa, int tick)
    {
        var result = new AgentActionLog();
        result = SetBasics(result, aa, tick) as AgentActionLog;
        result.Parameters = new List<ParameterLog>();
        foreach (var p in aa.Parameters)
        {
            result.Parameters.Add(ParameterLog.GetDebug(p, tick));
        }
        return result;
    }
}
