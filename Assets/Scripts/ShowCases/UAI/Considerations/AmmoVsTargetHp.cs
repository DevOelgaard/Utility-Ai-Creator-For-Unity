using System;
using ShowCases.UAI;

public class AmmoVsTargetHp: ConsiderationBoolean
{
    public AmmoVsTargetHp()
    {
        Description = "Fails if the agents ammo < target.HP + Extra";
        AddParameter("Extra", 1);
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);
        var extra =  ParameterContainer.GetParamInt("Extra").Value;
        var desiredAmmo = archer.targetHandler.ActiveTarget.HP + extra;

        return archer.CurrentAmmo >= desiredAmmo ? 1 : 0;
    }
}