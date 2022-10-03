using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine.PlayerLoop;

public abstract class ParamBase : RestoreAble
{
    public string Name;
    
    public IObservable<bool> OnValueChange => onValueChanged;
    protected readonly Subject<bool> onValueChanged = new Subject<bool>();
    
    protected override string GetFileName()
    {
        return Name;
    }

    public abstract string GetValueAsString();
    public abstract ParamBase Clone();
    public abstract Type GetValueType();
}

public abstract class ParamBase<T>: ParamBase
{
    private T v;
    public virtual T Value { 
        get => v;
        set
        {
            v = value;
            DebugService.Log("Setting parameter: " + Name + " To: " + v,this );
            onValueChanged.OnNext(true);
        }
    }
    protected ParamBase()
    {
        Name = "";
    }

    protected ParamBase(string name, T value)
    {
        Name = name;
        Value = value;
    }

    public override ParamBase Clone()
    {
        var clone = (ParamBase<T>)Activator.CreateInstance(GetType());
        clone.Name = Name;
        clone.Value = Value;
        return clone;
    }

    public override Type GetValueType()
    {
        return typeof(T);
    }

    internal override RestoreState GetState()
    {
        return new ParamBaseState<T>(this);
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var task = Task.Factory.StartNew(() =>
        {
            var state = (ParamBaseState<T>) s;
            Name = state.Name;
            v = state.Value;
        });
        await task;
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_Parameter);
    }
}

[Serializable]
public class ParamBaseState<T>: RestoreState
{
    public string Name;
    public T Value;
    public string ValueTypeString;
    public Type ValueType;

    public ParamBaseState(): base()
    {
    }

    public ParamBaseState(ParamBase<T> p): base(p)
    {
        Name = p.Name;
        ValueType = typeof(T);
        ValueTypeString = ValueType.ToString();
        Value = p.Value;
    }
}
