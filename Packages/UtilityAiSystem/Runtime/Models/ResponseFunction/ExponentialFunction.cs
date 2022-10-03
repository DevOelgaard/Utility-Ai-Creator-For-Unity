using System;
using System.Collections.Generic;
using UnityEngine;


public class ExponentialFunction : ResponseFunction
{
    public ExponentialFunction() : base(TypeToName.RF_Exponential) 
    {
        ParameterContainer.AddParameter("Power",2f);
    }

    protected override float CalculateResponseInternal(float x)
    {
        return Mathf.Pow(x,   ParameterContainer.GetParamFloat("Power").Value);
    }
}
