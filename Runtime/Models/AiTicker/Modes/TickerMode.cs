using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class TickerMode: RestoreAble
{
    internal UaiTickerMode Name;
    internal string Description;
    public ParameterContainer ParameterContainer;
    public Dictionary<string, Parameter>.ValueCollection Parameters => 
        ParameterContainer.Parameters;
    

    protected TickerMode(UaiTickerMode name, string description)
    {
        Description = description;
        Name = name;
        ParameterContainer = new ParameterContainer(GetParameters);
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
        if (state == null)
        {
            Name = default;
            Description = "Error state is null";
        } 
        else
        {
            Name = Enum.Parse<UaiTickerMode>(state.Name);
            Description = state.Description;
            ParameterContainer.RestoreFromState(state.ParameterContainerState);
            // var parameters =
            //     await RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
            // foreach (var parameter in parameters)
            // {
            //     ParameterContainer.AddParameter(parameter);
            // }
        }
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_TickerModes);
        await RestoreAbleService.SaveRestoreAblesToFile(Parameters
            .Where(p => p != null),path + "/" + Consts.FolderName_Parameters, persister);
    }
}

[Serializable]
public class TickerModeState: RestoreState
{
    public string Name;
    public string Description;
    // public List<string> Parameters;
    public ParameterContainerState ParameterContainerState;


    public TickerModeState()
    {
    }

    public TickerModeState(UaiTickerMode name, string description, List<Parameter> parameters, TickerMode o) : base(o)
    {
        Name = name.ToString();
        Description = description;
        // Parameters = RestoreAbleService.NamesToList(parameters);
        ParameterContainerState = o.ParameterContainer.GetState();
    }
}
