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

    public ErrorConsideration(ErrorConsideration original) : base(original)
    {
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    internal override AiObjectModel Clone()
    {
        return new ErrorConsideration(this);
    }
}