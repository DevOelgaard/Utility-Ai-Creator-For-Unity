using System;
using ShowCases.UAI;

public class ReloadContinuation: ConsiderationModifier
{
    public ReloadContinuation()
    {
        Description = "Sets the Parent weight, when the parent was selected last time. Until Ammo is full.";
        AddParameter("Weight", 1f);
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);
        if (archer.CurrentAmmo >= archer.AmmoCapacity)
        {
            return float.NaN;
        }
        if (context.LastSelectedBucket == context.CurrentEvaluatedBucket)
        {
            return ParameterContainer.GetParamFloat("Weight").Value;
        }
        return float.NaN;
    }

    public override float CalculateScore(IAiContext context)
    {
        return CalculateBaseScore(context);
    }
}