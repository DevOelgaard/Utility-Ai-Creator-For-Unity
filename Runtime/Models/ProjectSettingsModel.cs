using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;

internal class ProjectSettingsModel
{
    internal IObservable<string> OnCurrentProjectPathChanged => onCurrentProjectPathChanged;
    private readonly Subject<string> onCurrentProjectPathChanged = new Subject<string>();
    public string CurrentProjectPath
    {
        get => currentProjectPath;
        set
        {
            currentProjectPath = value;
            onCurrentProjectPathChanged.OnNext(currentProjectPath);
        }
    }
    private string currentProjectPath = "";
    public ProjectSettingsModel()
    {

    }
}