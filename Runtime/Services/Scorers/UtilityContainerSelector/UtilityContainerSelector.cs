using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class UtilityContainerSelector: RestoreAble, IIdentifier
{
    internal List<Parameter> Parameters;
    protected UtilityContainerSelector()
    {
        Parameters = GetParameters();
    }

    public abstract Bucket GetBestUtilityContainer(List<Bucket> containers, AiContext context);
    public abstract Decision GetBestUtilityContainer(List<Decision> containers, AiContext context);

    public abstract string GetDescription();

    public abstract string GetName();

    protected abstract List<Parameter> GetParameters();

    internal override RestoreState GetState()
    {
        return new UCSState(Parameters, this);
    }

    protected override void RestoreInternal(RestoreState state, bool restoreDebug = false)
    {
        var s = state as UCSState;
        Parameters = new List<Parameter>();
        foreach(var pS in s.Parameters)
        {
            var p = Restore<Parameter>(pS);
            Parameters.Add(p);
        }
    }
}

[Serializable]
public class UCSState: RestoreState
{
    public List<ParameterState> Parameters;

    public UCSState()
    {
    }

    public UCSState(List<Parameter> parameters, UtilityContainerSelector ucs): base(ucs)
    {
        Parameters = new List<ParameterState>();
        foreach(var p in parameters)
        {
            Parameters.Add((ParameterState)p.GetState());
        }
    }
}
