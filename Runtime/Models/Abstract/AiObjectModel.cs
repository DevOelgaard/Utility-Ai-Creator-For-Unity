using System;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using UniRxExtension;
using UnityEngine;

public abstract class AiObjectModel: RestoreAble
{
    internal AiObjectMetaData MetaData = new AiObjectMetaData();
    public readonly CompositeDisposable disposables = new CompositeDisposable();
    public string HelpText { get; protected set; }
    public IObservable<string> OnNameChanged => onNameChanged;
    private readonly Subject<string> onNameChanged = new Subject<string>();

    public IObservable<string> OnDescriptionChanged => onDescriptionChanged;
    private readonly Subject<string> onDescriptionChanged = new Subject<string>();

    internal IObservable<InfoModel> OnInfoChanged => onInfoChanged;
    private readonly Subject<InfoModel> onInfoChanged = new Subject<InfoModel>();

    public List<ScoreModel> ScoreModels = new List<ScoreModel>();
    // ReSharper disable once MemberCanBeProtected.Global
    public string ContextAddress { get; private set; }
    // ReSharper disable once MemberCanBePrivate.Global
    public readonly ParameterContainer ParameterContainer;
    public Dictionary<string, Parameter>.ValueCollection Parameters => ParameterContainer.Parameters;
    
    protected AiObjectModel(): base()
    {
        Name = StringService.SpaceBetweenUpperCase(GetType().ToString());
        ParameterContainer = new ParameterContainer(GetParameters);
        UpdateInfo();
    }

    protected virtual void UpdateInfo() { }
    protected override string GetFileName()
    {
        return Name;
    }

    protected void AddParameter(Parameter param)
    {
        ParameterContainer.AddParameter(param);
    }

    protected Parameter GetParameter(string parameterName)
    {
        return ParameterContainer.GetParameter(parameterName);
    }
    
    protected virtual List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    protected abstract AiObjectModel InternalClone();
    

    protected virtual void SetBaseClone(AiObjectModel clone)
    {
        clone.MetaData = new AiObjectMetaData
        {
            Type = GetType()
        };
        clone.Name = Name;
        clone.Description = Description;
        clone.HelpText = HelpText;
        foreach (var parameter in Parameters)
        {
            clone.AddParameter(parameter);
        }
    }
    
    internal AiObjectModel Clone()
    {
        var clone = InternalClone();
        SetBaseClone(clone);
        return clone;
    }

    internal async Task<AiObjectModel> CloneAsync()
    {
        var clone = await Task.Run(InternalClone);
        // var clone = InternalClone();
        SetBaseClone(clone);
        return clone;
    }
    internal virtual string GetUiName()
    {
        return Name + " (" + this.GetType().ToString() + ")";
    }
    public virtual string GetContextAddress(AiContext context)
    {
        return ContextAddress;
    }

    protected virtual string GetNameFormat(string currentName)
    {
        return currentName;
    }

    private string name;
    public string Name
    {
        get
        {
            if (string.IsNullOrEmpty(name))
            {
                Name = "Default";
            }
            return name;
        }
        set
        {
            name = GetNameFormat(value);
            onNameChanged.OnNext(Name);
        }
    }


    private string description;
    public string Description
    {
        get => description;
        set
        {
            if (value == description)
                return;
            description = value;
            onDescriptionChanged.OnNext(description);
        }
    }

    private InfoModel info;
    internal InfoModel Info 
    { 
        get => info; 
        set 
        {
            info = value;
            onInfoChanged.OnNext(info);
        } 
    }

    public virtual void SetContextAddress(string address)
    {
        ContextAddress = address;
    }

    protected virtual void ClearSubscriptions()
    {
        disposables.Clear();
    }

    ~AiObjectModel()
    {
        ClearSubscriptions();
    }
}
