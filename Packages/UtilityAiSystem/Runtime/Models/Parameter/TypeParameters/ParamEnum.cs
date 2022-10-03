using System;
using System.Globalization;
using System.Threading.Tasks;

public class ParamEnum: ParamBase<Enum>
{
    public Type EnumType;
    public ParamEnum()
    {
    }

    public ParamEnum(string name, Enum value) : base(name, value)
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
            var state = s as ParamEnumState;
            EnumType = state.ValueType;
            Value = (Enum)Enum.Parse(EnumType, state.CurrentSelection);
        });
        await task;
    }
}

public class ParamEnumState: ParamBaseState<Enum>
{
    public string CurrentSelection;
    public ParamEnumState()
    {
    }

    public ParamEnumState(ParamEnum p): base(p)
    {
        CurrentSelection = p.GetValueAsString();
    }
}
