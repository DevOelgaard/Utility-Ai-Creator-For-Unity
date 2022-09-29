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
        AddParameter("Base",1f);
    }

    protected override float CalculateResponseInternal(float x)
    {
        return Mathf.Log(x, ParameterContainer.GetParamFloat("Base").Value)/10/10;
    }
}