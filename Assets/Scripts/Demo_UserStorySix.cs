
using System;
using System.Collections.Generic;

public class Demo_UserStorySix: Consideration
{
    public Demo_UserStorySix()
    {
        AddParameter("OffSet",0.1f);
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var offSet = ParameterContainer.GetParamFloat("OffSet").Value;
        return UnityEngine.Time.deltaTime % 2 == 0 ? 0+offSet : 1-offSet;
    }
}