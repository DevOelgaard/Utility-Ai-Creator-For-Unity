using System;
using System.Collections.Generic;
using UnityEngine;


public class ExponentialFunction : ResponseFunction
{
    public ExponentialFunction() : base(TypeToName.RF_Exponential) {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter> {
            new Parameter("Power",2f),
            };
    }

    protected override float CalculateResponseInternal(float x)
    {
        return Mathf.Pow(x, Convert.ToSingle(Parameters[0].Value));
    }
}
