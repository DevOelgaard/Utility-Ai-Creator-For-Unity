using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UniRx;
using UnityEditor;

internal class ProjectSettingsService
{
    private CompositeDisposable modelChangedSubscription = new CompositeDisposable();
    internal IObservable<bool> OnProjectSettingsChanged => onProjectSettingsChanged;
    private readonly Subject<bool> onProjectSettingsChanged = new Subject<bool>();
    private readonly ProjectSettingsModel model;
    private readonly IPersister persister;
    private ProjectSettingsService()
    {
        persister = new JsonPersister();
        var loaded = persister.LoadObject<ProjectSettingsModel>(Consts.ProjectSettingsPath);
        model = loaded.Success ? loaded.LoadedObject : new ProjectSettingsModel();
        model.OnCurrentProjectPathChanged
            .Subscribe(_ => onProjectSettingsChanged.OnNext(true))
            .AddTo(modelChangedSubscription);
    }

    internal string GetCurrentProjectDirectory()
    {
        if (string.IsNullOrEmpty(model.CurrentProjectPath))
        {
            return "";
        }
        var directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(model.CurrentProjectPath) ?? string.Empty).FullName+"/";
        return directory;
    }

    internal string GetBackupDirectory()
    {
        return new DirectoryInfo(System.IO.Path.GetDirectoryName(GetProjectBackupPath()) ?? string.Empty).FullName+"/";
    }

    internal string GetCurrentProjectName(bool includeExtension = false) 
    {
        if (string.IsNullOrEmpty(model.CurrentProjectPath))
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
        return model.CurrentProjectPath;
    }

    internal string GetDirectory(string path)
    {
        if (string.IsNullOrEmpty(path)) return path;
        return new DirectoryInfo(System.IO.Path.GetDirectoryName(path) ?? string.Empty).FullName+"/";
    }
    
    public string GetProjectBackupPath()
    {
        return Consts.FileUasProjectBackUp + GetCurrentProjectName(true);
    }

    private void SetProjectPath(string path)
    {
        model.CurrentProjectPath = path;
        SaveSettings();
    }

    internal async Task CreateProject()
    {
        var path = EditorUtility.SaveFilePanel("New Project", "", "New Project", Consts.FileExtension_UasProject);

        SetProjectPath(path);
        await UasTemplateService.Instance.Reset();
        await UasTemplateService.Instance.Save();
        SaveSettings();
    }

    internal async Task SaveProjectAs()
    {
        var path = EditorUtility.SaveFilePanel("New Project", "", "New Project", Consts.FileExtension_UasProject);
        SetProjectPath(path);
        await UasTemplateService.Instance.Save();
        await UasTemplateService.Instance.LoadCurrentProject();
    }

    internal void LoadProject()
    {
        var filtes = new string[8];
        filtes[0] = "UAS Project";
        filtes[1] = Consts.FileExtension_UasProject;
        filtes[2] = "All Files";
        filtes[3] = "*";


        var path = EditorUtility.OpenFilePanelWithFilters("Open Project", "", filtes);
        SetProjectPath(path);
    }

    internal static ProjectSettingsService Instance
    {
        get { return _instance ??= new ProjectSettingsService(); }
    }
    private static ProjectSettingsService _instance;

    internal void SaveSettings()
    {
        persister.SaveObject(model, Consts.ProjectSettingsPath);
    }

    
    // https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/linq/how-to-compare-the-contents-of-two-folders-linq
    internal bool ProjectSaved()
    {
        // Current Project Directory
        var cpd = GetCurrentProjectDirectory();
        //Back Up Directory
        var bud = GetBackupDirectory();

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
