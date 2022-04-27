using System;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using UniRxExtension;
using UnityEngine;

public abstract class AiObjectModel: RestoreAble, IInitializeAble
{
    public Type BaseAiObjectType { get; protected set; }
    public ContextAddress ca;
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
    // ReSharper disable once MemberCanBePrivate.Global
    public ParameterContainer ParameterContainer { get; private set; }
    public Dictionary<string, Parameter>.ValueCollection Parameters => ParameterContainer.Parameters;
    public Guid Guid { get; protected set; }
    protected AiObjectModel(): base()
    {
        Guid = Guid.NewGuid();
        Name = StringService.SpaceBetweenUpperCase(GetType().ToString()) + "-Template";
        ParameterContainer = new ParameterContainer(GetParameters);
    }
    
    public virtual void Initialize()
    {
        DebugService.Log("Initializing: " + this.Name, this);
        ca = new ContextAddress(this);
        UpdateInfo();
    }

    protected virtual void UpdateInfo()
    {
        SetParent(ca.Parent,ca.Index);
    }
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

    protected override void OnRestoreComplete()
    {
        base.OnRestoreComplete();
        UpdateInfo();
    }

    protected virtual void SetBaseClone(AiObjectModel clone)
    {
        clone.MetaData = new AiObjectMetaData
        {
            Type = GetType()
        };
        clone.Name = Name;
        clone.Description = Description;
        clone.HelpText = HelpText;
        clone.CurrentDirectory = CurrentDirectory+"-Clone";
        clone.ParameterContainer = ParameterContainer.Clone();

    }

    protected virtual void OnCloneComplete()
    {
        
    }

    internal AiObjectModel Clone()
    {
        var clone = InternalClone();
        SetBaseClone(clone);
        clone.OnCloneComplete();
        return clone;
    }

    internal async Task<AiObjectModel> CloneAsync()
    {
        var clone = await Task.Run(InternalClone);
        SetBaseClone(clone);
        return clone;
    }
    internal virtual string GetUiName()
    {
        return Name + " (" + this.GetType().ToString() + ")";
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
            if (value == GetType().ToString())
            {
                value += "-Template";
            }
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

    public virtual void SetParent(AiObjectModel parent, int indexInParent)
    {
        ca.SetParentAndIndex(parent,indexInParent);
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
