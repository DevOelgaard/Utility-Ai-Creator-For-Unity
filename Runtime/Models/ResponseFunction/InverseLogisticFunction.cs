using System;
using System.Collections.Generic;
using UnityEngine;

// https://stackoverflow.com/questions/10097891/inverse-logistic-function-reverse-sigmoid-function
public class InverseLogisticFunction : ResponseFunction
{
    public InverseLogisticFunction() : base(TypeToName.RF_InverseLogistic)
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Base", 4f),
        };
    }

    protected override float CalculateResponseInternal(float x)
    {
        var baseLn = Convert.ToSingle(GetParameter("Base").Value);
        return ((float)Math.Log(x,baseLn) - (float)Math.Log(1-x,baseLn))/10 + 0.5f;
    }
}
