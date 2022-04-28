using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class LogarithmicFunction : ResponseFunction
{
    public LogarithmicFunction() : base(TypeToName.RF_Logarithmic)
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Base", 1f),
        };
    }

    protected override float CalculateResponseInternal(float x)
    {
        return Mathf.Log(x, Convert.ToSingle(GetParameter("Base").Value)/10)/10;
    }
}