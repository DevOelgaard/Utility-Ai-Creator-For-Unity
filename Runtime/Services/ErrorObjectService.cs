using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal static class ErrorObjectService
{
    internal static AiObjectModel GetErrorObject(Type t)
    {
        if (t.IsAssignableFrom(typeof(Consideration)))
        {
            return new Error_Consideration();
        }

        if (t.IsAssignableFrom(typeof(AgentAction)))
        {
            return new Error_Action();
        }

        else return (AiObjectModel)InstantiaterService.CreateInstance(t);
    }
}