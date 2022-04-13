using System;
using UniRx;
using System.Collections.Generic;
using System.Threading.Tasks;

public abstract class AiObjectModel: RestoreAble
{
    internal AiObjectMetaData MetaData = new AiObjectMetaData();
    private readonly CompositeDisposable disposables = new CompositeDisposable();
    public List<Parameter> Parameters = new List<Parameter>();
    public string HelpText { get; protected set; }
    public IObservable<string> OnNameChanged => onNameChanged;
    private readonly Subject<string> onNameChanged = new Subject<string>();

    public IObservable<string> OnDescriptionChanged => onDescriptionChanged;
    private readonly Subject<string> onDescriptionChanged = new Subject<string>();

    internal IObservable<InfoModel> OnInfoChanged => onInfoChanged;
    private readonly Subject<InfoModel> onInfoChanged = new Subject<InfoModel>();

    public List<ScoreModel> ScoreModels = new List<ScoreModel>();
    public string ContextAddress { get; private set; }
    protected AiObjectModel(): base()
    {
        Name = StringService.SpaceBetweenUpperCase(GetType().ToString());
        UpdateInfo();
    }

    protected virtual void UpdateInfo() { }
    protected override string GetFileName()
    {
        return Name;
    }

    protected abstract AiObjectModel InternalClone();

    protected virtual void SetBaseClone(AiObjectModel clone)
    {
        clone.MetaData = new AiObjectMetaData();
        clone.MetaData.Type = GetType();
        clone.Name = Name;
        clone.Description = Description;
        clone.HelpText = HelpText;
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

    protected class MainWindowModelState
    {
        public string Name;
        public string Description;

        public MainWindowModelState(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }


    private string name;
    public string Name
    {
        get
        {
            if (String.IsNullOrEmpty(name))
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
