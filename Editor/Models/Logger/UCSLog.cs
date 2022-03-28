using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class UCSLog: ILogModel
{
    internal string Name;
    internal List<ParameterLog> Parameters;

    internal static UCSLog GetDebug(UtilityContainerSelector ucs, int tick)
    {
        var result = new UCSLog();
        result.Name = ucs.GetName();
        result.Parameters = new List<ParameterLog>();
        foreach (var p in ucs.Parameters)
        {
            result.Parameters.Add(ParameterLog.GetDebug(p, tick));
        }
        return result;
    }
}
