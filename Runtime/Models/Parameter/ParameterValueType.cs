// using System;
// using System.ComponentModel;
// using System.Threading.Tasks;
// using UniRx;
// using UnityEngine;
//
// public class ParameterValueType: RestoreAble
// {
//     public string Name;
//     private ValueType v;
//     public ValueType Value { 
//         get => v;
//         set
//         {
//             v = value;
//             onValueChanged.OnNext(v);
//         }
//     }
//     public IObservable<object> OnValueChange => onValueChanged;
//     private readonly Subject<object> onValueChanged = new Subject<object>();
//
//     public ParameterValueType()
//     {
//     }
//     public ParameterValueType(string name, ValueType val)
//     {
//         Name = name;
//         Value = val;
//     }
//
//     public virtual ParameterValueType Clone()
//     {
//         return new ParameterValueType
//         {
//             Name = Name,
//             Value = Value
//         };
//     }
//
//     protected override string GetFileName()
//     {
//         return Name;
//     }
//
//     #region Persistence
//
//     internal override RestoreState GetState()
//     {
//         return new ParameterBaseState<T>(Name, this);
//     }
//
//     protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
//     {
//         var task = Task.Factory.StartNew(() =>
//         {
//             var state = (ParameterBaseState) s;
//             Name = state.Name;
//             Value = state.Value;
//         });
//         await task;
//     }
//
//     protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
//     {
//         await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_Parameter);
//     }
//     #endregion
// }
//
// public class ParameterBaseState: RestoreState
// {
//     public string Name;
//     public ValueType Value;
//     public string ValueTypeString;
//     public Type ValueType;
//     public float[] RGBA = new float[4];
//     
//     public ParameterBaseState(string name, ParameterValueType p): base(p)
//     {
//         Name = name;
//         ValueTypeString = p.Value.GetType().ToString();
//         ValueType = p.Value.GetType();
//         if (p.Value is Color color)
//         {
//             RGBA = new float[4];
//             RGBA[0] = color.r;
//             RGBA[1] = color.g;
//             RGBA[2] = color.b;
//             RGBA[3] = color.a;
//         } else
//         {
//             this.Value = p.Value;
//         }
//     }
// }