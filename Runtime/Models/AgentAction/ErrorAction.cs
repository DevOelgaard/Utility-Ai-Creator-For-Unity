using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ErrorAction : AgentAction
{
    public ErrorAction() : base()
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