using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public class Parameter: RestoreAble
{
    public string Name;
    private object v;
    public virtual object Value { 
        get => v;
        set
        {
            v = value;
            DebugService.Log("Setting parameter: " + Name + " To: " + v,this );
            onValueChanged.OnNext(v);
        }
    }

    public ParameterTypes ParameterEnum;

    public IObservable<object> OnValueChange => onValueChanged;
    private readonly Subject<object> onValueChanged = new Subject<object>();

    public Parameter()
    {
        Name = "";
    }

    public Parameter(string name, object value, ParameterTypes pEnum = ParameterTypes.None)
    {
        Name = name;
        Value = value;
        ParameterEnum = pEnum;
    }

    public virtual Parameter Clone()
    {
        // return AssetDatabaseService.DeepCopy(this);
        
        
        var clone = (Parameter)Activator.CreateInstance(GetType());
        clone.Name = Name;
        if (Value.GetType() != typeof(Color))
        {
            clone.Value = AssetService.DeepCopy(Value);
        }
        else
        {
            clone.Value = (Color)Value;
        }
        clone.ParameterEnum = ParameterEnum;
        return clone;
    }

    protected override string GetFileName()
    {
        return Name;
    }

    internal override RestoreState GetState()
    {
        return new ParameterState(Name, v, this);
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var task = Task.Factory.StartNew(() =>
        {
            var state = (ParameterState) s;
            Name = state.Name;
            if (state.ValueTypeString == "UnityEngine.Color")
            {
                v = new Color(state.RGBA[0], state.RGBA[1], state.RGBA[2], state.RGBA[3]);
            } else if (state.ValueType == typeof(float))
            {
                v = Convert.ToSingle(state.Value);
            } else if (state.ValueTypeString == "System.Double")
            {
                v = Convert.ToDouble(state.Value);
            }
            else
            {
                v = state.Value;
            }
        });
        await task;
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_Parameter);
    }
}

[Serializable]
public class ParameterState: RestoreState
{
    public string Name;
    public object Value;
    public string ValueTypeString;
    public Type ValueType;
    public float[] RGBA = new float[4];

    public ParameterState(): base()
    {
    }

    public ParameterState(string name, object v, Parameter p): base(p)
    {
        Name = name;
        ValueTypeString = v.GetType().ToString();
        ValueType = v.GetType();
        if (v is Color color)
        {
            RGBA = new float[4];
            RGBA[0] = color.r;
            RGBA[1] = color.g;
            RGBA[2] = color.b;
            RGBA[3] = color.a;
        } else
        {
            this.Value = v;
        }
    }
}
