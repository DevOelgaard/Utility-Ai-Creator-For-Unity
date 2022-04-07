using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class ConsiderationModifier: Consideration
{
    public ConsiderationModifier(): base()
    {
        IsModifier = true;
        HelpText = "Sets the weight of the current Utility Container to the returned value only the last modifier is valid.";
    }

    public override string GetTypeDescription()
    {
        return "Consideration Modifier";
    }

}