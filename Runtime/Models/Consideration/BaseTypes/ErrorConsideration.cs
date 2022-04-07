using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class ErrorConsideration : Consideration
{
    public ErrorConsideration() : base()
    {
        //Name = "";
        //Description = "";
        //HelpText = "";
    }
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    public override string GetTypeDescription()
    {
        return "Error Consideration";
    }
}