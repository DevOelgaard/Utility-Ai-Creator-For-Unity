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

internal class ProjectSettingsService
{
    private readonly CompositeDisposable modelChangedSubscription = new CompositeDisposable();
    internal IObservable<bool> OnProjectSettingsChanged => onProjectSettingsChanged;
    private readonly Subject<bool> onProjectSettingsChanged = new Subject<bool>();
    public readonly ProjectSettingsModel model;
    private readonly IPersister persister;

    internal static ProjectSettingsService Instance => _instance ??= new ProjectSettingsService();
    private static ProjectSettingsService _instance;
    private ProjectSettingsService()
    {
        persister = new JsonPersister();
        var loaded = PersistenceAPI.Instance
            .LoadObjectPath<ProjectSettingsModel>(Consts.ProjectSettingsPath);
        
        model = loaded.IsSuccessFullyLoaded ? loaded.LoadedObject : new ProjectSettingsModel();
        model.OnCurrentProjectPathChanged
            .Subscribe(_ => onProjectSettingsChanged.OnNext(true))
            .AddTo(modelChangedSubscription);
    }
    
    internal string GetCurrentProjectDirectory()
    {
        return model.CurrentProjectPath;
    }

    internal string GetTemporaryDirectory()
    {
        var backUpPath = GetProjectTemporaryPath();
        return new DirectoryInfo(Path.GetDirectoryName(backUpPath) ?? 
                                 string.Empty).FullName+"/";
    }

    internal string GetCurrentProjectName(bool includeExtension = false)
    {
        return model.CurrentProjectName;
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
    
    private string GetProjectTemporaryPath()
    {
        var currentProjectName = GetCurrentProjectName(true);
        return Consts.FileUasProjectTemp + currentProjectName;
    }

    private void SetProjectPath(string path)
    {
        model.CurrentProjectPath = path + "/";
        model.LastProjectDirectory = Directory.GetParent(path).ToString();
        DebugService.Log("Last Project Directory set to: " + model.LastProjectDirectory, this);
        SaveSettings();
    }

    internal async Task CreateProject()
    {
        var path = EditorUtility
            .SaveFolderPanel("New Project", model.LastProjectDirectory, "New Project");
        DebugService.Log("Creating new Project at path: " + path, this);

        SetProjectPath(path);
        SetProjectName(path);
        TemplateService.Instance.Reset();
        await TemplateService.Instance.Save();
    }

    private void SetProjectName(string path)
    {
        model.CurrentProjectName = new DirectoryInfo(path).Name;
        SaveSettings();
    }

    internal async Task SaveProjectAs()
    {
        var path = EditorUtility
            .SaveFolderPanel("New Project", model.LastProjectDirectory, "New Project");
        
        SetProjectPath(path);
        SetProjectName(path);

        await TemplateService.Instance.Save();
        await TemplateService.Instance.LoadCurrentProject();
    }

    internal void LoadProject()
    {
        var filters = new string[8];
        filters[0] = "UAS Project";
        filters[1] = Consts.FileExtension_TemplateService;
        filters[2] = "All Files";
        filters[3] = "*";


        var path = EditorUtility.OpenFolderPanel("Open Project",model.LastProjectDirectory,"UAI Project");
        if (path.Length == 0) return;
        SetProjectPath(path);
    }

    internal void SaveSettings()
    {
        persister.SaveObjectAsync(model, Consts.ProjectSettingsPath);
    }

    
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-compare-the-contents-of-two-folders-linq
    internal bool ProjectSaved()
    {
        // Current Project Directory
        var currentProjectDirectory = GetCurrentProjectDirectory();
        //Back Up Directory
        var temporaryDirectory =  GetTemporaryDirectory();

        if (string.IsNullOrEmpty(currentProjectDirectory) || string.IsNullOrEmpty(temporaryDirectory))
        {
            return false;
        }
        var cpdInfo = new DirectoryInfo(currentProjectDirectory);
        var budInfo = new DirectoryInfo(temporaryDirectory);

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
            DebugService.LogWarning("Could not validate if project was saved. Exception: " + e, this);
            return false;
        }
    }

    ~ProjectSettingsService()
    {
        modelChangedSubscription.Clear();
    }
}
