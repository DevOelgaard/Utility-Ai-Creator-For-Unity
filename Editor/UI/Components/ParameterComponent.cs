using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UniRx;
using UnityEngine;

public class ParameterComponent: VisualElement
{
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    public VisualElement field;
    public ParameterComponent()
    {

    }

    internal void UpdateUi(ParamBase parameter)
    {
        disposables.Clear();
        Clear();
        if (parameter.GetType() == typeof(ParamEnum))
        {
            var p = parameter as ParamEnum;
            var localField = new EnumField(parameter.Name);
            localField.Init(p.Value);
            localField.value = p.Value;
            localField.RegisterCallback<ChangeEvent<Enum>>(evt => p.Value = evt.newValue);
            parameter.OnValueChange
                .Subscribe(v =>
                {
                    localField.value = p.Value;
                })
                .AddTo(disposables);
            Add(localField);
            this.field = localField;
        } else
        {
            var t = parameter.GetValueType();
            if (t == typeof(double))
            {
                throw new NotImplementedException("ParamDouble not implemented. Use float instead");

                // parameter.Value = Convert.ToDouble(parameter.Value);
                // t = typeof(float);
            }
            if (t == typeof(int) || t == typeof(Int16) || t == typeof(Int32) || t == typeof(Int64))
            {
                var p = parameter as ParamInt;
                var localField = new IntegerFieldMinMax(parameter.Name)
                {
                    value = p.Value
                };
                localField.RegisterCallback<ChangeEvent<int>>(evt => p.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = p.Value;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(float) || t == typeof(Single))
            {
                var p = parameter as ParamFloat;
                var localField = new FloatFieldMinMax(parameter.Name)
                {
                    value = p.Value
                };
                localField.RegisterCallback<ChangeEvent<float>>(evt =>
                        p.Value = evt.newValue
                    );
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = p.Value;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(string))
            {
                var p = parameter as ParamString;
                var localField = new TextField(parameter.Name)
                {
                    value = p.Value
                };
                localField.RegisterCallback<ChangeEvent<string>>(evt => p.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = p.Value;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;

            }
            else if (t == typeof(Enum))
            {
                var p = parameter as ParamEnum;
                var localField = new TagField(parameter.Name)
                {
                    value = p.Value.ToString()
                };
                localField.RegisterCallback<ChangeEvent<ParameterTypes>>(evt => p.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = p.GetValueAsString();
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(long))
            {
                throw new NotImplementedException("ParamLong not implemented. Use float instead");
                // var localField = new LongField(parameter.Name)
                // {
                //     value = (long)parameter.Value
                // };
                // localField.RegisterCallback<ChangeEvent<long>>(evt => parameter.Value = evt.newValue);
                // parameter.OnValueChange
                //     .Subscribe(v =>
                //     {
                //         localField.value = (long)v;
                //     })
                //     .AddTo(disposables);
                // Add(localField);
                // this.field = localField;
            }
            else if (t == typeof(bool))
            {
                var p = parameter as ParamBool;
                var localField = new Toggle(parameter.Name)
                {
                    value = p.Value
                };
                localField.RegisterCallback<ChangeEvent<bool>>(evt => p.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = p.Value;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(Color))
            {
                var p = parameter as ParamColor;
                var localField = new ColorField(parameter.Name)
                {
                    value = p.Value
                };
                localField.RegisterCallback<ChangeEvent<Color>>(evt => p.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = p.Value;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(List<string>))
            {

            }
        }
        
    }

    ~ParameterComponent()
    {
        disposables.Clear();
    }
}
