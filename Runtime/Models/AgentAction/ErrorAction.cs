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

    public ErrorAction(ErrorAction original) : base(original)
    {
    }

    internal override AiObjectModel Clone()
    {
        return new ErrorAction(this);
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

}