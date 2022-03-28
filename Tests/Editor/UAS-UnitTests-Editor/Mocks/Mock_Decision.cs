using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mock_Decision: Decision
{
    public float ReturnValue { get; private set; }

    public Mock_Decision(float returnValue = 0f)
    {
        ReturnValue = returnValue;
        LastCalculatedUtility = returnValue;
        var a = new Mock_AgentAction();
        a.Name = returnValue.ToString();
        AgentActions.Add(a);
    }

    protected override float CalculateUtility(AiContext context) {
        return ReturnValue;
    }
}
