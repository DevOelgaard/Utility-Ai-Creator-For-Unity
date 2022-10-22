using UnityEngine;

public class Demo_PrintTxt: AgentAction
{
    public Demo_PrintTxt()
    {
        // Add parameters in the constructor
        AddParameter("Text to print","");
    }

    public override void OnStart(IAiContext context)
    {
        // Read the value of the parameter defined above. The parameterName is case sensitive
        var textFromParameter = ParameterContainer.GetParamString("Text to print").Value;
        
        // Get the agent from the Context and cast him as AgentMono
        // Don't think to much about the casting to AgentMono, just do it ;)
        var agent = context.Agent as AgentMono;
        
        // Printing to the console
        Debug.Log(agent.name +": " + textFromParameter);
    }
}