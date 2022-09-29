using System.Threading.Tasks;
using UnityEngine;

public class ParamColor: ParamBase<Color>
{
    public ParamColor()
    {
    }

    public ParamColor(string name, Color value) : base(name, value)
    {
    }

    public override string GetValueAsString()
    {
        return Value.ToString();
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        await base.RestoreInternalAsync(s, restoreDebug);

        var task = Task.Factory.StartNew(() =>
        {
            var state = s as ParamColorState;
            Value = new Color(state.RGBA[0], state.RGBA[1], state.RGBA[2], state.RGBA[3]);
        });
        await task;
    }
}

public class ParamColorState: ParamBaseState<Color>
{
    public float[] RGBA = new float[4];

    public ParamColorState()
    {
    }

    public ParamColorState(ParamColor p): base(p)
    {
        RGBA = new float[4];
        RGBA[0] = p.Value.r;
        RGBA[1] = p.Value.g;
        RGBA[2] = p.Value.b;
        RGBA[3] = p.Value.a;
    }
}