using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class UtilityContainerSelector: RestoreAble, IIdentifier
{
    public readonly ParameterContainer ParameterContainer;

    protected UtilityContainerSelector()
    {
        ParameterContainer = new ParameterContainer(GetParameters);
    }
    public void AddParameter(Parameter param)
    {
        ParameterContainer.AddParameter(param);
    }

    protected Parameter GetParameter(string parameterName)
    {
        return ParameterContainer.GetParameter(parameterName);
    }

    public abstract Bucket GetBestUtilityContainer(List<Bucket> containers, IAiContext context);
    public abstract Decision GetBestUtilityContainer(List<Decision> containers, IAiContext context);

    public abstract string GetDescription();

    public abstract string GetName();

    protected abstract List<Parameter> GetParameters();

    internal override RestoreState GetState()
    {
        return new UtilityContainerSelectorState(ParameterContainer.Parameters.ToList(), this);
    }

    protected override string GetFileName()
    {
        return GetName();
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var state = s as UtilityContainerSelectorState;
        var parameters = await RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
        foreach (var parameter in parameters)
        {
            AddParameter(parameter);
        }
    }

    protected override async Task InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        await persister.SaveObjectAsync(state, path + "." + Consts.FileExtension_UtilityContainerSelector);
        await RestoreAbleService.SaveRestoreAblesToFile(ParameterContainer.Parameters.Where(p => p != null),path + "/" + Consts.FolderName_Parameters, persister);
    }
}

[Serializable]
public class UtilityContainerSelectorState: RestoreState
{
    public List<string> Parameters;
    public UtilityContainerSelectorState()
    {
    }

    public UtilityContainerSelectorState(List<Parameter> parameters, UtilityContainerSelector ucs): base(ucs)
    {
        Parameters = ucs.ParameterContainer.Parameters.Select(p => p.Name).ToList();
    }
}
