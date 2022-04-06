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
            return new ErrorConsideration();
        }

        if (t.IsAssignableFrom(typeof(AgentAction)))
        {
            return new ErrorAction();
        }

        else return (AiObjectModel)InstantiaterService.Instance.CreateInstance(t);
    }
}