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
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new ParameterEnum("Enum param", UtilityContainerTypes.Decision)
        };
    }

}