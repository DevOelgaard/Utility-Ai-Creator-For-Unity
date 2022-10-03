public class ParamBool: ParamBase<bool>
{
    public ParamBool()
    {
    }

    public ParamBool(string name, bool value) : base(name, value)
    {
    }

    public override string GetValueAsString()
    {
        return Value.ToString();
    }
}