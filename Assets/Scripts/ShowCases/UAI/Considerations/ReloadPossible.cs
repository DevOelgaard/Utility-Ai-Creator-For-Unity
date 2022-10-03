using ShowCases.UAI;

public class ReloadPossible: ConsiderationBoolean
{
    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);

        return archer.transform.localPosition.x == 0 ? 1 : 0;
    }
}