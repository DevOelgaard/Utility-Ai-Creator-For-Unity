using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UtilityContainerSelector: PersistSingleFile, IIdentifier
{
    public ParameterContainer ParameterContainer { get; protected set; }

    protected UtilityContainerSelector()
    {
        ParameterContainer = new ParameterContainer();
    }

    public abstract Bucket GetBestUtilityContainer(List<Bucket> containers, IAiContext context);
    public abstract Decision GetBestUtilityContainer(List<Decision> containers, IAiContext context);

    public abstract string GetDescription();

    public abstract string GetName();

    public UtilityContainerSelector Clone()
    {
        var clone = AssetService.GetInstanceOfType<UtilityContainerSelector>(GetType().ToString());
        clone.ParameterContainer = ParameterContainer.Clone();
        return clone;
    }

    public override SingleFileState GetSingleFileState()
    {
        return new UtilityContainerSelectorSingleFileState(this);
    }

    protected override async Task RestoreFromFile(SingleFileState state)
    {
        var s = state as UtilityContainerSelectorSingleFileState;
        if (s.ParameterContainerState == null)
        {
            ParameterContainer = new ParameterContainer();
        }
        else
        {
            ParameterContainer.RestoreFromState(s.ParameterContainerState);
        }
    }
}

[Serializable]
public class UtilityContainerSelectorSingleFileState: SingleFileState
{
    public ParameterContainerState ParameterContainerState;

    public UtilityContainerSelectorSingleFileState()
    {
    }

    public UtilityContainerSelectorSingleFileState(UtilityContainerSelector o): base(o)
    {
        ParameterContainerState = o.ParameterContainer.GetState();
    }
}

// [Serializable]
// public class UtilityContainerSelectorState: RestoreState
// {
//     public List<string> Parameters;
//     public UtilityContainerSelectorState()
//     {
//     }
//
//     public UtilityContainerSelectorState(List<Parameter> parameters, UtilityContainerSelector ucs): base(ucs)
//     {
//         Parameters = ucs.ParameterContainer.Parameters.Select(p => p.Name).ToList();
//     }
// }
