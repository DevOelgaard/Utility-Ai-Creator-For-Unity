using ShowCases.UAI;
using UnityEditor;

public class ReloadAmount: Consideration
{
    public ReloadAmount()
    {
        Description = "Returns the percentage of ammo, which would be filled by reloading";
        MinFloat.Value = 0f;
        MaxFloat.Value = 1f;}

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);
        return (float)archer.ReloadAmount / archer.AmmoCapacity;
    }
}