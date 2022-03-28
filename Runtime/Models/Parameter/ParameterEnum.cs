using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class ParameterEnum: Parameter
{
    public Enum CurrentSelction => Value as Enum;
    public Type EnumType;

    public ParameterEnum() : base()
    {
    }

    public ParameterEnum(string name, Enum currentValue): base(name, currentValue)
    {
        EnumType = currentValue.GetType();
    }

    internal override RestoreState GetState()
    {
        return new ParameterEnumState(Name, CurrentSelction, EnumType, this);
    }

    protected override void RestoreInternal(RestoreState s, bool restoreDebug = false)
    {
        base.RestoreInternal(s, restoreDebug);
        var state = s as ParameterEnumState;
        EnumType = Type.GetType(state.EnumType);
        Value = Enum.Parse(EnumType, state.CurrentEnumSelection);
    }
}

public class ParameterEnumState : ParameterState
{
    public string CurrentEnumSelection;
    public string EnumType;

    public ParameterEnumState()
    {
    }

    public ParameterEnumState(string name, Enum currentSelection, Type t, ParameterEnum p): base(name, currentSelection, p)
    {
        CurrentEnumSelection = currentSelection.ToString();
        EnumType = t.ToString();
    }
}
