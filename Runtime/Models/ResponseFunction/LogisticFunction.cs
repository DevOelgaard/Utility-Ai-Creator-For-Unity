using System;
using System.Collections.Generic;
using UnityEngine;


// https://stackoverflow.com/questions/412019/math-optimization-in-c-sharp
// https://en.wikipedia.org/wiki/Logistic_function
public class LogisticFunction : ResponseFunction
{
    public LogisticFunction() : base(TypeToName.RF_Logistic)
    {
        AddParameter("Growth Rate", 10f);
        AddParameter("Mid Point", 0.5f);
    }

    protected override float CalculateResponseInternal(float x)
    {
        // L / 1 + e^-k(x-x0)
        return Convert.ToSingle(Max.Value) / (1.0f + Mathf.Exp(-ParameterContainer.GetParamFloat("Growth Rate").Value * 
                                                                      (x - ParameterContainer.GetParamFloat("Mid Point").Value)));
    }
}
