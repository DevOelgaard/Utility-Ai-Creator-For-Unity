using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class TickerMode: PersistSingleFile
{
    internal UaiTickerMode Name;
    internal string Description;
    public ParameterContainer ParameterContainer;
    // TODO Change this
    // public Dictionary<string, Parameter>.ValueCollection Parameters => 
    //     ParameterContainer.Parameters;
    

    protected TickerMode(UaiTickerMode name, string description)
    {
        Description = description;
        Name = name;
        ParameterContainer = new ParameterContainer();
    }

    internal abstract void Tick(List<IAgent> agents, TickMetaData metaData);
    internal virtual void Tick(IAgent agent, TickMetaData metaData)
    {
        agent.Tick(metaData);
    }

    protected override async Task RestoreFromFile(SingleFileState state)
    {
        var s = state as TickerModeSingleFileState;
        if (s == null)
        {
            Name = default;
            Description = "Error state is null";
        } 
        else
        {
            Name = Enum.Parse<UaiTickerMode>(s.Name);
            Description = s.Description;
            ParameterContainer.RestoreFromState(s.ParameterContainerState);
        }
    }

    public override SingleFileState GetSingleFileState()
    {
        return new TickerModeSingleFileState(this);
    }
}

[Serializable]
public class TickerModeSingleFileState: SingleFileState
{
    public string Name;
    public string Description;
    public ParameterContainerState ParameterContainerState;


    public TickerModeSingleFileState()
    {
    }

    public TickerModeSingleFileState(TickerMode o) : base(o)
    {
        Name = o.Name.ToString();
        Description = o.Description;
        ParameterContainerState = o.ParameterContainer.GetState();
    }
}