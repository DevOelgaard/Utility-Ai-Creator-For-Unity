using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UnityEditor;

internal class ProjectSettingsService
{
    private ProjectSettingsModel model;
    private IPersister persister;
    private ProjectSettingsService()
    {
        persister = new JSONPersister();
        var loaded = persister.LoadObject<ProjectSettingsModel>(Consts.ProjectSettingsPath);
        if (loaded.Success)
        {
            model = loaded.LoadedObject;
        } else
        {
            model = new ProjectSettingsModel();
        }
    }

    internal string GetCurrentProjectDirectory()
    {
        if (string.IsNullOrEmpty(model.CurrentProjectPath))
        {
            return "";
        }
        var dírectory = new DirectoryInfo(System.IO.Path.GetDirectoryName(model.CurrentProjectPath)).FullName+"/";
        return dírectory;
        //return Path.GetDirectoryName(model.CurrentProjectPath+"/");
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

    internal void SetProjectPath(string path)
    {
        model.CurrentProjectPath = path;
        SaveSettings();
    }

    internal void CreateProject()
    {
        var path = EditorUtility.SaveFilePanel("New Project", "", "New Project", Consts.FileExtension_UasProject);

        SetProjectPath(path);
        UASTemplateService.Instance.Reset();
        UASTemplateService.Instance.Save();
        SaveSettings();
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
        get
        {
            if (instance == null)
            {
                instance = new ProjectSettingsService();
            }
            return instance;
        }
    }
    private static ProjectSettingsService instance;

    internal void SaveSettings()
    {
        persister.SaveObject(model, Consts.ProjectSettingsPath);
    }
}
