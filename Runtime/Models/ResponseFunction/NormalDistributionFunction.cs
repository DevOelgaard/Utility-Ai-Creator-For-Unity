using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class NormalDistributionFunction : ResponseFunction
{
    public NormalDistributionFunction() : base(TypeToName.RF_NormalDistribution)
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Mean", 0.5f),
            new Parameter("Std Deviation", 0.1f),
        };
    }

    protected override float CalculateResponseInternal(float x)
    {
        var mean = Convert.ToSingle(Parameters[0].Value);
        var stdDeviation = Convert.ToSingle(Parameters[1].Value);
        var f = (1 / (stdDeviation * Mathf.Sqrt(2 * Mathf.PI))) * Math.E;
        var p = (float)-0.5 * Mathf.Pow(((x - mean) / stdDeviation), 2);
        var result = Mathf.Pow((float)f, p);
        
        return result;
    }
}