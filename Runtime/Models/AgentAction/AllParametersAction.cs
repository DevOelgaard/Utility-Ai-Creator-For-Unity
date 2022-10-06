using UnityEngine;

public class AllParametersAction: AgentAction
{
    public AllParametersAction()
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
}