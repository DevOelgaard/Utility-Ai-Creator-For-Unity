using ShowCases.UAI;
using UnityEngine;

public class DistanceToReload: Consideration
{
    public DistanceToReload()
    {
        MaxFloat.Value = 6f;
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);
        var field = archer.field;
        var distance = Mathf.Abs(archer.transform.localPosition.x);

        return distance;
    }
}