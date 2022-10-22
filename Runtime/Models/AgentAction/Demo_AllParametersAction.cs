using UnityEngine;

public class Demo_AllParametersAction: AgentAction
{
    public Demo_AllParametersAction()
    {
        HelpText = "A spawn point must be set in the AiContext " +
           "A Unit type must spawning must either be selected in parameters or in code";
        AddParameter("Bool", true);
        AddParameter("Enum",PerformanceTag.High);
        AddParameter("Float",1f);
        AddParameter("Int",100);
        AddParameter("String","Hi you");
        AddParameter("Color",Color.blue);
    }

    public override void OnStart(IAiContext context)
    {
        var b = ParameterContainer.GetParamBool("Bool");
        var c = ParameterContainer.GetParamColor("Color");
        var e = ParameterContainer.GetParamEnum("Enum");
        var f = ParameterContainer.GetParamFloat("Float");
        var i = ParameterContainer.GetParamInt("Int");
        var s = ParameterContainer.GetParamString("String");
    }
}

