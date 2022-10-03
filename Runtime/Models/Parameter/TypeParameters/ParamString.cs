using System.Globalization;

public class ParamString: ParamBase<string>
{
    public ParamString()
    {
    }

    public ParamString(string name, string value) : base(name, value)
    {
    }
    public override string GetValueAsString()
    {
        return Value;
    }
}