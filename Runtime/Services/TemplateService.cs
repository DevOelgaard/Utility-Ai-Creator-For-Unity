using UnityEngine;
using UniRx;
using UniRxExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;

internal class TemplateService: RestoreAble
{
    private readonly CompositeDisposable subscriptions = new CompositeDisposable();
    private Dictionary<string, ReactiveList<AiObjectModel>> collectionsByLabel = new Dictionary<string, ReactiveList<AiObjectModel>>();
    public ReactiveListNameSafe<AiObjectModel> AIs;
    public ReactiveListNameSafe<AiObjectModel> Buckets;
    public ReactiveListNameSafe<AiObjectModel> Decisions;
    public ReactiveListNameSafe<AiObjectModel> Considerations;
    public ReactiveListNameSafe<AiObjectModel> AgentActions;
    public ReactiveListNameSafe<AiObjectModel> ResponseCurves;
    private string stateString = "";
    internal IObservable<string> OnStateChanged => onStateChanged;
    private readonly Subject<string> onStateChanged = new Subject<string>();

    private string loadedPath = "";
    internal bool isLoaded;
    internal IObservable<bool> OnLoadComplete => onLoadComplete;
    private readonly Subject<bool> onLoadComplete = new Subject<bool>();

    public IObservable<ReactiveList<AiObjectModel>> OnCollectionChanged => onCollectionChanged;
    private readonly Subject<ReactiveList<AiObjectModel>> onCollectionChanged = new Subject<ReactiveList<AiObjectModel>>();

    private string projectDirectory;

    private bool includeDemos = true;
    public bool IncludeDemos
    {
        get => includeDemos;
        set
        {
            includeDemos = value; 
            onIncludeDemosChanged?.OnNext(value);
        }
    }
    public IObservable<bool> OnIncludeDemosChanged => onIncludeDemosChanged;
    private readonly Subject<bool> onIncludeDemosChanged = new Subject<bool>();

    private static TemplateService _instance;
    internal static TemplateService Instance => _instance ??= new TemplateService();
    private System.Guid id;

    private TemplateService()
    {
        Init(true);
        id = Guid.NewGuid();
        DebugService.Log("ID set: " + id, this);
        // EditorApplication.playModeStateChanged += _instance.SaveFromStateChange;
    }

    private void Init(bool restore)
    {
        DebugService.Log("Instantiating",this);

        
        AIs = new ReactiveListNameSafe<AiObjectModel>();
        Buckets = new ReactiveListNameSafe<AiObjectModel>();
        Decisions = new ReactiveListNameSafe<AiObjectModel>();
        Considerations = new ReactiveListNameSafe<AiObjectModel>();
        AgentActions = new ReactiveListNameSafe<AiObjectModel>();
        ResponseCurves = new ReactiveListNameSafe<AiObjectModel>();

        collectionsByLabel = new Dictionary<string, ReactiveList<AiObjectModel>>
        {
            {Consts.Label_UAIModel, AIs},
            {Consts.Label_BucketModel, Buckets},
            {Consts.Label_DecisionModel, Decisions},
            {Consts.Label_ConsiderationModel, Considerations},
            {Consts.Label_AgentActionModel, AgentActions},
            {Consts.Label_ResponseCurve, ResponseCurves}
        };
        

        SubscribeToCollectionChanges();

        if (restore)
        {
            AsyncHelpers.RunSync(LoadBackup);
            DebugService.Log("Instantiation complete with restore",this);
        }
        else
        {
            DebugService.Log("Instantiation complete no restore",this);
        }
    }

    private void SetState(string s)
    {
        stateString = s;
        onStateChanged.OnNext(stateString);
    }

    private void SubscribeToCollectionChanges()
    {
        foreach(var (_, value) in collectionsByLabel)
        {
            value.OnValueChanged
                .Subscribe(_ => 
                    onCollectionChanged.OnNext(value))
                .AddTo(subscriptions);
        }
    }

    internal async Task LoadBackup()
    {
        await LoadCurrentProject(true);
    }

    internal async Task LoadCurrentProject(bool backup = false)
    {
        SetState("Loading");
        var loadPath = backup
            ? ProjectSettingsService.Instance.GetProjectBackupPath()
            : ProjectSettingsService.Instance.GetCurrentProjectPath();
        DebugService.Log("Loading path: " + loadPath,this);

        if (loadPath == null)
        {
            DebugService.Log("Failed to load", this);
            ClearCollectionNotify();
            
            return;
        }

        if (loadedPath == loadPath)
        {
            DebugService.Log("Reloading path: " + loadPath, this);
        }
        projectDirectory = ProjectSettingsService.Instance.GetDirectory(loadPath);
        DebugService.Log("ProjectDirectory: " + projectDirectory, this);


        if (string.IsNullOrEmpty(projectDirectory))
        {
            SetState("");
            ClearCollectionNotify();
            return;
        }
        
        var perstistAPI = PersistenceAPI.Instance;
        
        var state = await perstistAPI.LoadObjectPathAsync<UasTemplateServiceState>(loadPath);
        if (state.LoadedObject == null)
        {
            ClearCollectionNotify();
            SubscribeToCollectionChanges();
            DebugService.LogError("Loading failed state.LoadedObjet == null",this);
            DebugService.LogError("State.ErrorMessage: " + state.ErrorMessage,this);
        }
        else
        {
            try
            {
                await RestoreAsync(state.LoadedObject);
                DebugService.Log("Restore Complete AIs: " + AIs.Values.Count,this);

                isLoaded = true;
                loadedPath = loadPath;
                onLoadComplete.OnNext(true);
                DebugService.Log("Load complete",this);
                SetState("");
            }
            catch (Exception ex)
            {
                DebugService.LogError("Loading failed: " + ex, this);
                SetState("Error");
                throw new Exception("UAS Template Service Restore Failed : ", ex);
            }
        }
    }
    

    internal async Task Save(bool backup = false)
    {
        var currentState = stateString;
        SetState("Saving");
        DebugService.Log("Saving Backup: " + backup, this);
        var path = !backup
            ? ProjectSettingsService.Instance.GetCurrentProjectDirectory()
            : ProjectSettingsService.Instance.GetBackupDirectory();


        DebugService.Log("Saving path: " + path, this);

        var perstistAPI = PersistenceAPI.Instance;
        var currentProjectName = ProjectSettingsService.Instance.GetCurrentProjectName(true);
        await perstistAPI.SaveDestructiveObjectPathAsync(this, path, currentProjectName);
        SetState(currentState);
    }

    protected override string GetFileName()
    {
        return ProjectSettingsService.Instance.GetCurrentProjectName();
    }

    internal void Reset()
    {
        subscriptions.Clear();
        ClearCollectionNoNotify();
        Init(false);
    }

    internal ReactiveList<AiObjectModel> GetCollection(string label)
    {
        if (collectionsByLabel.ContainsKey(label))
        {
            return collectionsByLabel[label];
        } else
        {
            return null;
        }
    }

    internal ReactiveList<AiObjectModel> GetCollection(Type t)
    {
        if (t.IsAssignableFrom(typeof(Ai)))
        {
            return AIs;
        }
        if (t.IsAssignableFrom(typeof(Bucket)))
        {
            return Buckets;
        }
        if (t.IsAssignableFrom(typeof(Decision)))
        {
            return Decisions;
        }
        if (t.IsAssignableFrom(typeof(Consideration)))
        {
            return Considerations;
        }

        if (t.IsAssignableFrom(typeof(AgentAction)))
        {
            return AgentActions;
        }

        if (t.IsAssignableFrom(typeof(ResponseCurve)))
        {
            return ResponseCurves;
        }
        return null;
    }



    internal override RestoreState GetState()
    {
        return new UasTemplateServiceState(AIs, Buckets, Decisions, Considerations, AgentActions, this);
    }
    

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        DebugService.Log("Saving Ais: " + AIs.Values.Count, this);
        var directoryPath = Path.GetDirectoryName(path);
        if (!path.Contains(Consts.FileExtension_UasProject))
        {
            path += "." + Consts.FileExtension_UasProject;
        }
        await persister.SaveObjectAsync(state, path) ;

        persister = new JsonPersister();
        var tasks = new List<Task>()
        {
            RestoreAbleService.SaveRestoreAblesToFile(AIs.Values.Cast<Ai>(),
                directoryPath + "/" + Consts.FolderName_Ais, persister),
            RestoreAbleService.SaveRestoreAblesToFile(Buckets.Values.Cast<Bucket>(),
                directoryPath + "/" + Consts.FolderName_Buckets, persister),
            RestoreAbleService.SaveRestoreAblesToFile(Decisions.Values.Cast<Decision>(),
                directoryPath + "/" + Consts.FolderName_Decisions, persister),
            RestoreAbleService.SaveRestoreAblesToFile(Considerations.Values.Cast<Consideration>(),
                directoryPath + "/" + Consts.FolderName_Considerations, persister),
            RestoreAbleService.SaveRestoreAblesToFile(AgentActions.Values.Cast<AgentAction>(),
                directoryPath + "/" + Consts.FolderName_AgentActions, persister),
            RestoreAbleService.SaveRestoreAblesToFile(ResponseCurves.Values.Cast<ResponseCurve>(),
                directoryPath + "/" + Consts.FolderName_ResponseCurves, persister),
        };
        await Task.WhenAll(tasks);
        DebugService.Log("Saving Complete", this);
    }

    private async Task RestoreAsync(UasTemplateServiceState state)
    {
        await RestoreInternalAsync(state);
    }

    internal void Add(AiObjectModel model)
    {
        var collection = GetCollection(model);
        collection.Add(model);
    }

    internal void Remove(AiObjectModel model)
    {
        var collection = GetCollection(model);
        collection.Remove(model);
    }

    private void ClearCollectionNotify()
    {
        subscriptions?.Clear();
        AIs?.Clear();
        Buckets?.Clear();
        Decisions?.Clear();
        Considerations?.Clear();
        AgentActions?.Clear();
        ResponseCurves?.Clear();
    }

    private void ClearCollectionNoNotify() {
        subscriptions?.Clear();
        AIs?.ClearNoNotify();
        Buckets?.ClearNoNotify();
        Decisions?.ClearNoNotify();
        Considerations?.ClearNoNotify();
        AgentActions?.ClearNoNotify();
        ResponseCurves?.ClearNoNotify();
    }

    private ReactiveList<AiObjectModel> GetCollection(AiObjectModel model)
    {
        var type = model.GetType();

        if (TypeMatches(type, typeof(Consideration)))
        {
            return Considerations;
        }
        else if (TypeMatches(type,typeof(Decision)))
        {
            return Decisions;
        } else if (TypeMatches(type, typeof(AgentAction)))
        {
            return AgentActions;
        } else if (TypeMatches(type, typeof(Bucket)))
        {
            return Buckets;
        } else if (TypeMatches(type, typeof(Ai)))
        {
            return AIs;
        } else if (TypeMatches(type, typeof(ResponseCurve)))
        {
            return ResponseCurves;
        }
        return null;
    }
    
    private bool TypeMatches(Type a, Type b)
    {
        return a.IsAssignableFrom(b) || a.IsSubclassOf(b);
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        DebugService.Log("Start Restore: " + s.FileName, this);
        ClearCollectionNoNotify();
        SubscribeToCollectionChanges();
        var state = (UasTemplateServiceState)s;
        var tasks = new List<Task>
        {
            RestoreAbleService
                .LoadObjectsAndSortToCollection<Ai>(projectDirectory + Consts.FolderName_Ais, 
                    state.aIs,AIs,restoreDebug),
            RestoreAbleService
                .LoadObjectsAndSortToCollection<Bucket>(projectDirectory + Consts.FolderName_Buckets, 
                    state.buckets,Buckets,restoreDebug),
            RestoreAbleService
                .LoadObjectsAndSortToCollection<Decision>(projectDirectory + Consts.FolderName_Decisions, 
                    state.decisions,Decisions,restoreDebug),
            RestoreAbleService
                .LoadObjectsAndSortToCollection<Consideration>(projectDirectory + Consts.FolderName_Considerations,
                    state.considerations,Considerations,restoreDebug),
            RestoreAbleService
                .LoadObjectsAndSortToCollection<AgentAction>(projectDirectory + Consts.FolderName_AgentActions, 
                    state.agentActions,AgentActions,restoreDebug),
            RestoreAbleService
                .LoadObjectsAndSortToCollection<ResponseCurve>(projectDirectory + Consts.FolderName_ResponseCurves, 
                    state.responseCurves,ResponseCurves,restoreDebug)
        };

        await Task.WhenAll(tasks);
        DebugService.Log("Finished Restore: " + s.FileName, this);

        onCollectionChanged.OnNext(AIs);
    }
    
    ~TemplateService()
    {
        DebugService.Log("Destroying",this);
        subscriptions.Clear();
    }
}

[Serializable]
public class UasTemplateServiceState : RestoreState
{
    public List<string> aIs;
    public List<string> buckets;
    public List<string> decisions;
    public List<string> considerations;
    public List<string> agentActions;
    public List<string> responseCurves;
    public UasTemplateServiceState()
    {
    }

    internal UasTemplateServiceState(
        ReactiveList<AiObjectModel> aiS,
        ReactiveList<AiObjectModel> buckets, ReactiveList<AiObjectModel> decisions,
        ReactiveList<AiObjectModel> considerations, ReactiveList<AiObjectModel> agentActions, TemplateService model) : base(model)
    {
        aIs = RestoreAbleService.NamesToList(aiS.Values);

        this.buckets = RestoreAbleService.NamesToList(buckets.Values);

        this.decisions = RestoreAbleService.NamesToList(decisions.Values);

        this.considerations = RestoreAbleService.NamesToList(considerations.Values);

        this.agentActions = RestoreAbleService.NamesToList(agentActions.Values);

        responseCurves = RestoreAbleService.NamesToList(model.ResponseCurves.Values);
    }
}
