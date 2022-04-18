using System;
using System.Threading.Tasks;
using UniRx;
using UnityEngine.UIElements;

public class ParameterType<T>: RestoreAble
{
    public string Name;
    protected object v;
    public virtual object Value { 
        get => v;
        set
        {
            v = value;
            onValueChanged.OnNext(v);
        }
    }
    public IObservable<object> OnOnValueChange => onValueChanged;
    private readonly Subject<object> onValueChanged = new Subject<object>();

    public ParameterType()
    {
    }

    public ParameterType(string name, T value)
    {
        
    }

    protected override string GetFileName()
    {
        throw new System.NotImplementedException();
    }

    protected override Task RestoreInternalAsync(RestoreState state, bool restoreDebug = false)
    {
        throw new System.NotImplementedException();
    }

    protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        throw new System.NotImplementedException();
    }

    internal override RestoreState GetState()
    {
        throw new System.NotImplementedException();
    }
}

public class ParameterTypeState<T>: RestoreState
{

    public string Name;
    public T Value;
    
    public ParameterTypeState() : base()
    {
        
    }

    public ParameterTypeState(ParameterType<T> p) : base(p)
    {
        
    }
}