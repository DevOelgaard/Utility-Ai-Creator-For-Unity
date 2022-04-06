using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Mock_AgentAction : AgentAction
{


    public Mock_AgentAction(Mock_AgentAction original) : base(original)
    {
    }

    public Mock_AgentAction()
    {
    }

    internal override AiObjectModel Clone()
    {
        return new Mock_AgentAction(this);
    }
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}
