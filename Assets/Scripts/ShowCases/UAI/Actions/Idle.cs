using System.Collections.Generic;
using ShowCases.UAI;
using UnityEngine;

public class Idle: AgentAction
{
    public Idle()
    {
        Description = "Stands Idle and prints text";
        AddParameter("Text", "Text to print");
    }

    public override void OnStart(IAiContext context)
    {
        Act(context);
    }

    public override void OnGoing(IAiContext context)
    {
        Act(context);
    }

    private void Act(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);
        var text = ParameterContainer.GetParamString("Text").Value;
        archer.Idle(text);
    }
}