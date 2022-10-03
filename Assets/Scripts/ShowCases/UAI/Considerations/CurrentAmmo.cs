using ShowCases.UAI;
using UnityEditor;

public class CurrentAmmo: Consideration
{
    public CurrentAmmo()
    {
        Description = "Returns percentage of ammo in stock.";
        MinFloat.Value = 0f;
        MaxFloat.Value = 1f;
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);
        if (archer.CurrentAmmo == 0) return 0;
        return (float)archer.CurrentAmmo / archer.AmmoCapacity;
    }
}