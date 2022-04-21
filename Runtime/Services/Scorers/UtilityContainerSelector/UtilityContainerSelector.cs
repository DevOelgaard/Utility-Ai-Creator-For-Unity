using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UtilityContainerSelector: RestoreAble, IIdentifier
{
    protected readonly Dictionary<string, Parameter> ParametersByName = new Dictionary<string, Parameter>();
    public Dictionary<string, Parameter>.ValueCollection Parameters
    {
        get
        {
            if (!parametersInitialized)
            {
                InitializeParameters();
            }

            return ParametersByName.Values;
        }
    }
    private bool parametersInitialized = false;
    protected UtilityContainerSelector()
    {
    }
    public void AddParameter(Parameter param)
    {
        if (ParametersByName.ContainsKey(param.Name))
        {
            ParametersByName[param.Name] = param;
        }
        else
        {
            ParametersByName.Add(param.Name, param);
        }
    }

    protected Parameter GetParameter(string parameterName)
    {
        if (!parametersInitialized)
        {
            InitializeParameters();
        }
        
        if (!ParametersByName.ContainsKey(parameterName))
        {
            var p = Parameters.FirstOrDefault(p => p.Name == parameterName);
            if (p == null)
            {
                Debug.LogError("Couldn't find parameter: " + parameterName);
            }
            ParametersByName.Add(parameterName,p);
        }

        return ParametersByName[parameterName];
    }

    private void InitializeParameters()
    {
        if (parametersInitialized) return;
        foreach (var param in GetParameters())
        {
            ParametersByName.Add(param.Name, param);
        }
        parametersInitialized = true;
    }
    public abstract Bucket GetBestUtilityContainer(List<Bucket> containers, AiContext context);
    public abstract Decision GetBestUtilityContainer(List<Decision> containers, AiContext context);

    public abstract string GetDescription();

    public abstract string GetName();

    protected abstract List<Parameter> GetParameters();

    internal override RestoreState GetState()
    {
        return new UCSState(Parameters.ToList(), this);
    }

    protected override string GetFileName()
    {
        return GetName();
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var state = s as UCSState;
        var parameters = await RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
        foreach (var parameter in parameters)
        {
            AddParameter(parameter);
        }
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObject(state, path + "." + Consts.FileExtension_UtilityContainerSelector);
        await RestoreAbleService.SaveRestoreAblesToFile(Parameters.Where(p => p != null),path + "/" + Consts.FolderName_Parameters, persister);
    }
}

[Serializable]
public class UCSState: RestoreState
{
    public List<string> Parameters;
    public UCSState()
    {
    }

    public UCSState(List<Parameter> parameters, UtilityContainerSelector ucs): base(ucs)
    {
        Parameters = ucs.Parameters.Select(p => p.Name).ToList();
    }
}
