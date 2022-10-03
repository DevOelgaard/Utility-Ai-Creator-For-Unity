using ShowCases.UAI;
using UnityEditor;

public class ChanceToHit: Consideration
{
    public ChanceToHit()
    {
        Description = "Affected by distance to target and archer precision";
        MinFloat.Value = 0f;
        MaxFloat.Value = 1f;
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);

        return archer.GetCurrentChanceToHit();
    }
}