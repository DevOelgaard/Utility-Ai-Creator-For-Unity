using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Demo_PrintEnum : AgentAction
{
    public Demo_PrintEnum() : base()
    {
        //Name = "";
        //Description = "";
        //HelpText = "";
        ParameterContainer.AddParameter("Enum param", UtilityContainerTypes.Decision);
    }
}