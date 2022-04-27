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

    internal void UpdateUi(Parameter parameter)
    {
        disposables.Clear();
        Clear();
        if (parameter.GetType() == typeof(ParameterEnum))
        {
            var p = parameter as ParameterEnum;
            var localField = new EnumField(parameter.Name);
            localField.Init(p.CurrentSelction);
            localField.value = p.CurrentSelction;
            localField.RegisterCallback<ChangeEvent<Enum>>(evt => parameter.Value = evt.newValue);
            parameter.OnValueChange
                .Subscribe(v =>
                {
                    localField.value = (Enum)v;
                })
                .AddTo(disposables);
            Add(localField);
            this.field = localField;
        } else
        {
            var t = parameter.Value.GetType();
            if (t == typeof(double))
            {
                parameter.Value = (float)parameter.Value;
                t = typeof(float);
            }
            if (t == typeof(int) || t == typeof(Int16) || t == typeof(Int32) || t == typeof(Int64))
            {
                var localField = new IntegerFieldMinMax(parameter.Name)
                {
                    value = Convert.ToInt32(parameter.Value)
                };
                localField.RegisterCallback<ChangeEvent<int>>(evt => parameter.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = (int)v;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(float) || t == typeof(Single))
            {
                var localField = new FloatFieldMinMax(parameter.Name)
                {
                    value = (float)parameter.Value
                };
                localField.RegisterCallback<ChangeEvent<float>>(evt =>
                        parameter.Value = evt.newValue
                    );
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = (float)v;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(string) && parameter.ParameterEnum == ParameterTypes.None)
            {
                var localField = new TextField(parameter.Name)
                {
                    value = (string)parameter.Value
                };
                localField.RegisterCallback<ChangeEvent<string>>(evt => parameter.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = (string)v;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;

            }
            else if (parameter.ParameterEnum == ParameterTypes.Tag)
            {
                var localField = new TagField(parameter.Name)
                {
                    value = (string)parameter.Value
                };
                localField.RegisterCallback<ChangeEvent<ParameterTypes>>(evt => parameter.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = (string)v;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(long))
            {
                var localField = new LongField(parameter.Name)
                {
                    value = (long)parameter.Value
                };
                localField.RegisterCallback<ChangeEvent<long>>(evt => parameter.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = (long)v;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(bool))
            {
                var localField = new Toggle(parameter.Name)
                {
                    value = (bool)parameter.Value
                };
                localField.RegisterCallback<ChangeEvent<bool>>(evt => parameter.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = (bool)v;
                    })
                    .AddTo(disposables);
                Add(localField);
                this.field = localField;
            }
            else if (t == typeof(Color))
            {
                var localField = new ColorField(parameter.Name)
                {
                    value = (Color)parameter.Value
                };
                localField.RegisterCallback<ChangeEvent<Color>>(evt => parameter.Value = evt.newValue);
                parameter.OnValueChange
                    .Subscribe(v =>
                    {
                        localField.value = (Color)v;
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
