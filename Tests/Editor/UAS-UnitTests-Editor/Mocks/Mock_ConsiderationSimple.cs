using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mock_ConsiderationSimple : Consideration
{
    public float ReturnValue = 0f;

    public Mock_ConsiderationSimple()
    {
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        return ReturnValue;
    }

    public override float CalculateScore(AiContext context)
    {
        return ReturnValue;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
