public class ParamInt: ParamBase<int>
{
    public ParamInt()
    {
    }

    public ParamInt(string name, int value) : base(name, value)
    {
    }

    public override string GetValueAsString()
    {
        return Value.ToString();
    }
}