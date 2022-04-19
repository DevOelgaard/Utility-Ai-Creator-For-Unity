using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class UtilityContainerSelector: RestoreAble, IIdentifier
{
    internal List<Parameter> Parameters;
    protected UtilityContainerSelector()
    {
        Parameters = GetParameters();
    }

    public abstract Bucket GetBestUtilityContainer(List<Bucket> containers, AiContext context);
    public abstract Decision GetBestUtilityContainer(List<Decision> containers, AiContext context);

    public abstract string GetDescription();

    public abstract string GetName();

    protected abstract List<Parameter> GetParameters();

    internal override RestoreState GetState()
    {
        return new UCSState(Parameters, this);
    }

    protected override string GetFileName()
    {
        return GetName();
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var state = s as UCSState;
        var parameters = await RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
        Parameters = RestoreAbleService.SortByName(state.Parameters, parameters);
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
        Parameters = RestoreAbleService.NamesToList(ucs.Parameters);
    }
}
