using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class Error_Consideration : Consideration
{
    public Error_Consideration() : base()
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