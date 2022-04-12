using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Use this to set the weight of a certain Utility container
/// I.e: The agent must continue to eat untill full so the weight is set higher than normal
/// but still lower than the decision to defend himself
/// </summary>
public abstract class ConsiderationModifier: Consideration
{
    public ConsiderationModifier(): base()
    {
        IsModifier = true;
        IsScorer = false;
        HelpText = "Sets the weight of the current Utility Container to the returned value only the last modifier is valid.";
    }

    public override string GetTypeDescription()
    {
        return "Consideration Modifier";
    }

}