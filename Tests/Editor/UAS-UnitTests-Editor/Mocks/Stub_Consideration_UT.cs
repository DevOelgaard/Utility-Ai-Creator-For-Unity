using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Stub_Consideration_UT : Consideration
{
    public float ReturnValue;
    public Stub_Consideration_UT(float returnValue, List<Parameter> parameters)
    {
        ReturnValue = returnValue;

        Parameters = parameters;
        CurrentResponseCurve = new Mock_ResponseCurve("Mock");
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        return ReturnValue;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
