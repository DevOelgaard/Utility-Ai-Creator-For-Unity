using UnityEngine;

public class Demo_ConsiderationBoolean: ConsiderationBoolean
{
    public Demo_ConsiderationBoolean(): base()
    {
        Description = "Returns true/false with 50% chance";
    }

    protected override float CalculateBaseScore(IAiContext context)
    {
        var result = Random.Range(0, 100);
        return result < 50 ? 0 : 1;
    }
}