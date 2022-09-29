using System.Globalization;

public class ParamFloat: ParamBase<float>
{
    public ParamFloat()
    {
    }

    public ParamFloat(string name, float value) : base(name, value)
    {
    }

    public override string GetValueAsString()
    {
        return Value.ToString(CultureInfo.InvariantCulture);
    }
}