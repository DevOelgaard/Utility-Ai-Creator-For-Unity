public class LinearFunction : ResponseFunction
{
    public LinearFunction() : base(TypeToName.RF_Linear)
    {
        ParameterContainer.AddParameter("a", 1f);
        ParameterContainer.AddParameter("b", 0f);
    }

    protected override float CalculateResponseInternal(float x)
    {
        return ParameterContainer.GetParamFloat("a").Value * x + ParameterContainer.GetParamFloat("b").Value;
    }
}
