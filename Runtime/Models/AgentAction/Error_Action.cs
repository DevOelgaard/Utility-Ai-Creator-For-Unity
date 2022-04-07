using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Error_Action : AgentAction
{
    public Error_Action() : base()
    {
        //Name = "";
        //Description = "";
        //HelpText = "";
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}