using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

internal class Demo_ConsiderationRandomValue : Consideration
{

    public Demo_ConsiderationRandomValue(Demo_ConsiderationRandomValue original) : base(original)
    {
    }

    public Demo_ConsiderationRandomValue(): base()
    {
    }

    internal override AiObjectModel Clone()
    {
        return new Demo_ConsiderationRandomValue(this);
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        return UnityEngine.Random.Range(Convert.ToSingle(MinFloat.Value), Convert.ToSingle(MaxFloat.Value));
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
