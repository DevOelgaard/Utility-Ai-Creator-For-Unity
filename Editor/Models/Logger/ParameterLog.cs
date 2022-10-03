﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ParameterLog: ILogModel
{
    public string Name;
    public string Value;

    internal static ParameterLog GetDebug(ParamBase p, int tick)
    {
        var result = new ParameterLog();
        result.Name = p.Name;
        result.Value = p.GetValueAsString();
        return result;
    }
}
