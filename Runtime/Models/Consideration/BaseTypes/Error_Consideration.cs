using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal sealed class Error_Consideration : Consideration
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

    protected override float CalculateBaseScore(AiContext context)
    {
        return -1;
    }

    public override string GetTypeDescription()
    {
        return "Error Consideration";
    }
}