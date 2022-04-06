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

    public Demo_Setter(Demo_Setter original) : base(original)
    {
    }

    internal override AiObjectModel Clone()
    {
        return new Demo_Setter(this);
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