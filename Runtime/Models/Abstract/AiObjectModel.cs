using System;
using UniRx;
using System.Collections.Generic;

public abstract class AiObjectModel: RestoreAble
{
    internal AiObjectMetaData MetaData = new AiObjectMetaData();
    protected CompositeDisposable Disposables = new CompositeDisposable();
    public string HelpText { get; protected set; }
    public IObservable<string> OnNameChanged => onNameChanged;
    private Subject<string> onNameChanged = new Subject<string>();

    public IObservable<string> OnDescriptionChanged => onDescriptionChanged;
    private Subject<string> onDescriptionChanged = new Subject<string>();

    internal IObservable<InfoModel> OnInfoChanged => onInfoChanged;
    private Subject<InfoModel> onInfoChanged = new Subject<InfoModel>();

    public List<ScoreModel> ScoreModels = new List<ScoreModel>();

    protected AiObjectModel()
    {
        UpdateInfo();
    }

    protected virtual void UpdateInfo() { }

    internal abstract AiObjectModel Clone();

    internal virtual string GetUiName()
    {
        return Name + " (" + this.GetType().ToString() + ")";
    }
    internal virtual string GetContextAddress(AiContext context)
    {
        return Name;
    }

    public virtual string GetNameFormat(string name)
    {
        return name;
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

    protected virtual void ClearSubscriptions()
    {
        Disposables.Clear();
    }

    ~AiObjectModel()
    {
        ClearSubscriptions();
    }
}
