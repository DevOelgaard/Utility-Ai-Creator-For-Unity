using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class TickerMode: RestoreAble
{
    internal AiTickerMode Name;
    internal string Description;
    protected readonly Dictionary<string, Parameter> ParametersByName = new Dictionary<string, Parameter>();
    public Dictionary<string, Parameter>.ValueCollection Parameters => ParametersByName.Values;
    private bool parametersInitialized = false;

    protected TickerMode(AiTickerMode name, string description)
    {
        Description = description;
        Name = name;
    }


    protected override string GetFileName()
    {
        return TypeDescriptor.GetClassName(this);
    }

    internal abstract List<Parameter> GetParameters();
    internal abstract void Tick(List<IAgent> agents, TickMetaData metaData);
    internal virtual void Tick(IAgent agent, TickMetaData metaData)
    {
        agent.Tick(metaData);
    }

    internal override RestoreState GetState()
    {
        return new TickerModeState(Name, Description, Parameters.ToList(), this);
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var state = s as TickerModeState;
        Name = Enum.Parse<AiTickerMode>(state.Name);
        Description = state.Description;
        var parameters =
            await RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
        foreach (var parameter in parameters)
        {
            AddParameter(parameter);
        }
    }
    protected void AddParameter(Parameter param)
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
            foreach (var param in GetParameters())
            {
                ParametersByName.Add(param.Name, param);
            }
            parametersInitialized = true;
        }
        
        if (!ParametersByName.ContainsKey(parameterName))
        {
            var p = Parameters.FirstOrDefault(p => p.Name == parameterName);
            if (p == null)
            {
                DebugService.LogError("Couldn't find parameter: " + parameterName, this);
            }
            ParametersByName.Add(parameterName,p);
        }

        return ParametersByName[parameterName];
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path+"." + Consts.FileExtension_TickerModes);
        await RestoreAbleService.SaveRestoreAblesToFile(Parameters.Where(p => p != null),path + "/" + Consts.FolderName_Parameters, persister);
        // foreach (var parameter in Parameters)
        // {
        //     var subPath = path + "/" + Consts.FolderName_Parameters;
        //     parameter.SaveToFile(subPath, persister);
        // }
    }
}

[Serializable]
public class TickerModeState: RestoreState
{
    public string Name;
    public string Description;
    public List<string> Parameters;


    public TickerModeState()
    {
    }

    public TickerModeState(AiTickerMode name, string description, List<Parameter> parameters, TickerMode o) : base(o)
    {
        Name = name.ToString();
        Description = description;
        Parameters = RestoreAbleService.NamesToList(parameters);
    }
}
