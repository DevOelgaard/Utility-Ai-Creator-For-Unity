using System;
using UniRx;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoreLinq;
using UniRxExtension;
using UnityEngine;

public abstract class AiObjectModel: PersistSingleFile, IInitializeAble
{
    public Type BaseAiObjectType { get; protected set; }

    public ContextAddress ContextAddress
    {
        get
        {
            if (contextAddress == null)
            {
                Initialize();
            }

            return contextAddress;
        }
        private set => contextAddress = value;
    }

    public AiObjectModel Parent => ContextAddress.Parent;

    private ContextAddress contextAddress;
    internal AiObjectMetaData MetaData = new AiObjectMetaData();
    protected readonly CompositeDisposable disposables = new CompositeDisposable();
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
    // TODO Initialize this from parameterContainer
    public List<ParamBase> Parameters => ParameterContainer.Parameters;
    // public Dictionary<string, Parameter>.ValueCollection Parameters => ParameterContainer.Parameters;

    public Guid Guid { get; protected set; }
    protected AiObjectModel(): base()
    {
        Guid = Guid.NewGuid();
        Name = StringService.SpaceBetweenUpperCase(GetType().ToString());
        ParameterContainer = new ParameterContainer();
    }
    
    public virtual void Initialize()
    {
        DebugService.Log("Initializing: " + this.Name, this);
        ContextAddress = new ContextAddress(this);
        UpdateInfo();
    }
    public override string GetTypeDescription()
    {
        return GetType().ToString();
    }

    protected virtual void UpdateInfo()
    {
        SetParent(ContextAddress.Parent,ContextAddress.Index);
    }

    protected void AddParameter(string parameterName, object value)
    {
        ParameterContainer.AddParameter(parameterName, value);
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
            // if (value == GetType().ToString())
            // {
            //     value += "-Template";
            // }
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
        ContextAddress.SetParentAndIndex(parent,indexInParent);
    }

    protected virtual void ClearSubscriptions()
    {
        disposables.Clear();
    }

    // protected override async Task RestoreInternalAsync(RestoreState state, bool restoreDebug = false)
    // {
    //     var s = state as AiObjectState;
    //     ParameterContainer.RestoreFromState(s.ParameterContainerState);
    //     Guid = Guid.Parse(s.guid);
    // }

    protected override async Task RestoreFromFile(SingleFileState state)
    {
        var s = state as AiObjectModelSingleFileState;
        ParameterContainer.RestoreFromState(s.ParameterContainerState);
        Guid = Guid.Parse(s.guid);
        Name = s.Name;
        Description = s.Description;
        await RestoreInternalFromFile(state);
    }

    protected abstract Task RestoreInternalFromFile(SingleFileState state);

    ~AiObjectModel()
    {
        ClearSubscriptions();
    }
}

// public abstract class AiObjectState : RestoreState
// {
//     public ParameterContainerState ParameterContainerState;
//     public string guid;
//
//     protected AiObjectState(): base()
//     {
//     }
//
//     protected AiObjectState(AiObjectModel o) : base(o)
//     {
//         ParameterContainerState = o.ParameterContainer.GetState();
//         guid = o.Guid.ToString();
//     }
// }

public abstract class AiObjectModelSingleFileState : SingleFileState
{
    public ParameterContainerState ParameterContainerState;
    public string guid;
    public string Name;
    public string Description;

    protected AiObjectModelSingleFileState(): base()
    {
    }

    protected AiObjectModelSingleFileState(AiObjectModel o) : base(o)
    {
        ParameterContainerState = o.ParameterContainer.GetState();
        guid = o.Guid.ToString();
        Name = o.Name;
        Description = o.Description;
    }
}
