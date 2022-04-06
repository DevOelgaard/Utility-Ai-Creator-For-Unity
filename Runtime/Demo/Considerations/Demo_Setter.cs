using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Demo_Setter : Consideration
{
    public Demo_Setter() : base()
    {
        IsSetter = true;
        //Name = "";
        //Description = "";
        //HelpText = "";
    }

    protected override float CalculateBaseScore(AiContext context)
    {
        throw new NotImplementedException();
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }
}