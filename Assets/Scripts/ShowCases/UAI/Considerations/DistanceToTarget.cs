using ShowCases.UAI;

public class DistanceToTarget: Consideration
{
    public DistanceToTarget()
    {
        MinFloat.Value = 0f;
        MaxFloat.Value = 12f;
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);

        var target = archer.targetHandler.ActiveTarget;
        return target.GetDistance(archer.transform);

        // return target.GetDistanceNormalized(archer.transform.localPosition.x);
    }
}