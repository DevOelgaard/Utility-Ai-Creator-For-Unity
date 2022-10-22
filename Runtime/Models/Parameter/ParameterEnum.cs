// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;
//
// public class ParameterEnum: Parameter
// {
//     public Enum CurrentSelection => Value as Enum;
//     public Type EnumType;
//
//     public ParameterEnum() : base()
//     {
//     }
//
//     public ParameterEnum(string name, Enum currentValue): base(name, currentValue)
//     {
//         EnumType = currentValue.GetType();
//     }
//
//     internal override RestoreState GetState()
//     {
//         return new ParameterEnumState(Name, CurrentSelection, EnumType, this);
//     }
//
//     protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
//     {
//         await base.RestoreInternalAsync(s, restoreDebug);
//
//         var task = Task.Factory.StartNew(() =>
//         {
//             var state = s as ParameterEnumState;
//             EnumType = state.EnumType;
//             Value = Enum.Parse(EnumType, state.CurrentEnumSelection);
//         });
//         await task;
//     }
//
//     public override Parameter Clone()
//     {
//         var clone = (ParameterEnum)Activator.CreateInstance(GetType());
//         clone.Name = Name;
//         clone.Value = Value;
//         clone.ParameterEnum = ParameterEnum;
//         clone.EnumType = EnumType;
//         return clone;
//
//     }
// }
//
// public class ParameterEnumState : ParameterState
// {
//     public string CurrentEnumSelection;
//     public Type EnumType;
//
//     public ParameterEnumState()
//     {
//     }
//
//     public ParameterEnumState(string name, Enum currentSelection, Type t, ParameterEnum p): base(name, currentSelection, p)
//     {
//         CurrentEnumSelection = currentSelection.ToString();
//         EnumType = t;
//     }
// }
