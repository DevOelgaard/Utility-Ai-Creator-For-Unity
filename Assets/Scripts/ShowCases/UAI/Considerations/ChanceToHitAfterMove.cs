using System;
using ShowCases.UAI;
using UnityEditor;

public class ChanceToHitAfterMove: Consideration
{
    public ChanceToHitAfterMove()
    {
        Description = "Returns the ToHit improvement if after moving.";
        MinFloat.Value = 0f;
        MaxFloat.Value = 1f;
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);

        var currentTarget = archer.targetHandler.ActiveTarget;
        var distanceToTarget = currentTarget.GetDistance(archer.transform);
        var currentChanceToHit = archer.CalculateChanceToHit(distanceToTarget);
        
        var expectedPositionAfterMove = archer.GetDirection(false);
        var expectedDistanceToTarget = currentTarget.GetDistance(expectedPositionAfterMove);
        var expectedChanceToHit = archer.CalculateChanceToHit(expectedDistanceToTarget);
        if (Math.Abs(distanceToTarget - expectedDistanceToTarget) < 0.001)
        {
            return 0;
        }
        // return expectedChanceToHit-currentChanceToHit;
        return expectedChanceToHit;
    }
}