using ShowCases.UAI;

public class AmmoInStock: ConsiderationBoolean
{
    protected override float CalculateBaseScore(IAiContext context)
    {
        var archer = UAIHelper.GetArcherFromContext(context);
        return archer.CurrentAmmo > 0 ? 1 : 0;
    }
}