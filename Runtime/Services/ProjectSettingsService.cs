using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using UniRx;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
internal class ProjectSettingsService
{
    private readonly CompositeDisposable modelChangedSubscription = new CompositeDisposable();
    internal IObservable<bool> OnProjectSettingsChanged => onProjectSettingsChanged;
    private readonly Subject<bool> onProjectSettingsChanged = new Subject<bool>();
    private ProjectSettingsModel model;
    private IPersister persister;
    internal static ProjectSettingsService Instance;
    // static ProjectSettingsService()
    // {
    //
    // }

    [InitializeOnLoadMethod]
    static async void Init()
    {
        Instance = new ProjectSettingsService
        {
            persister = new JsonPersister()
        };
        var loaded = await PersistenceAPI.Instance
            .LoadObjectPath<ProjectSettingsModel>(Consts.ProjectSettingsPath);
        
        Instance.model = loaded.Success ? loaded.LoadedObject : new ProjectSettingsModel();
        Instance.model.OnCurrentProjectPathChanged
            .Subscribe(_ => Instance.onProjectSettingsChanged.OnNext(true))
            .AddTo(Instance.modelChangedSubscription);
    }
    
    internal string GetCurrentProjectDirectory()
    {
        if (model == null || string.IsNullOrEmpty(model.CurrentProjectPath))
        {
            return "";
        }
        return new DirectoryInfo(Path.GetDirectoryName(model.CurrentProjectPath) ?? 
                                          string.Empty).FullName+"/";
    }

    internal string GetBackupDirectory()
    {
        var backUpPath = GetProjectBackupPath();
        return new DirectoryInfo(Path.GetDirectoryName(backUpPath) ?? 
                                 string.Empty).FullName+"/";
    }

    internal string GetCurrentProjectName(bool includeExtension = false) 
    {

        if (model == null || string.IsNullOrEmpty(model.CurrentProjectPath))
        {
            return "No Project";
        }
        if (includeExtension)
        {
            return Path.GetFileName(model.CurrentProjectPath);
        } else
        {
            var path = Path.GetFileName(model.CurrentProjectPath);
            return path.Substring(0, path.IndexOf('.'));
        }
    }

    internal string GetCurrentProjectPath()
    {
        return model != null ? model.CurrentProjectPath : "";
    }

    internal string GetDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;
        return new DirectoryInfo(Path.GetDirectoryName(path) ?? string.Empty).FullName+"/";
    }
    
    public string GetProjectBackupPath()
    {
        var currentProjectName = GetCurrentProjectName(true);
        return Consts.FileUasProjectBackUp + currentProjectName;
    }

    private void SetProjectPath(string path)
    {
        model.CurrentProjectPath = path;
        SaveSettings();
    }

    internal async Task CreateProject()
    {
        var path = EditorUtility
            .SaveFilePanel("New Project", "", "New Project", 
                Consts.FileExtension_UasProject);

        SetProjectPath(path);
        await UasTemplateService.Instance.Reset();
        await UasTemplateService.Instance.Save();
        SaveSettings();
    }

    internal async Task SaveProjectAs()
    {
        var path = EditorUtility
            .SaveFilePanel("New Project", "", "New Project", 
                Consts.FileExtension_UasProject);
        
        SetProjectPath(path);
        await UasTemplateService.Instance.Save();
        await UasTemplateService.Instance.LoadCurrentProject();
    }

    internal void LoadProject()
    {
        var filters = new string[8];
        filters[0] = "UAS Project";
        filters[1] = Consts.FileExtension_UasProject;
        filters[2] = "All Files";
        filters[3] = "*";


        var path = EditorUtility.OpenFilePanelWithFilters("Open Project", "", filters);
        SetProjectPath(path);
    }

    internal void SaveSettings()
    {
        persister.SaveObject(model, Consts.ProjectSettingsPath);
    }

    
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-compare-the-contents-of-two-folders-linq
    internal async Task<bool> ProjectSaved()
    {
        // Current Project Directory
        var cpd = GetCurrentProjectDirectory();
        //Back Up Directory
        var bud =  GetBackupDirectory();

        if (string.IsNullOrEmpty(cpd) || string.IsNullOrEmpty(bud))
        {
            return false;
        }
        var cpdInfo = new DirectoryInfo(cpd);
        var budInfo = new DirectoryInfo(bud);

        var fileComparer = new FileComparer();

        try
        {
            var cpdFiles = cpdInfo.GetFiles("*.uas*", SearchOption.AllDirectories)
                .Where(f => !f.Name.EndsWith(".meta"))
                .ToList();
            var budFiles = budInfo.GetFiles("*.uas*", SearchOption.AllDirectories)
                .Where(f => !f.Name.EndsWith(".meta"))
                .ToList();
            
            return cpdFiles.SequenceEqual(budFiles, fileComparer);
        }
        catch (Exception e)
        {
            if (e.GetType() == typeof(DirectoryNotFoundException))
            {
                return false;
            }
            throw;
        }
    }

    ~ProjectSettingsService()
    {
        modelChangedSubscription.Clear();
    }
}
