using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class TickerMode: RestoreAble
{
    internal AiTickerMode Name;
    internal string Description;
    internal List<Parameter> Parameters;

    protected TickerMode(AiTickerMode name, string description)
    {
        Description = description;
        Name = name;
        Parameters = GetParameters();
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
        return new TickerModeState(Name, Description, Parameters, this);
    }

    protected override async Task RestoreInternalAsync(RestoreState s, bool restoreDebug = false)
    {
        var task = Task.Factory.StartNew(() =>
        {
            var state = s as TickerModeState;
            Name = Enum.Parse<AiTickerMode>(state.Name);
            Description = state.Description;
            var parameters =
                RestoreAbleService.GetParameters(CurrentDirectory + Consts.FolderName_Parameters, restoreDebug);
            Parameters = RestoreAbleService.SortByName(state.Parameters, parameters);
        });
        await task;
    }

    protected override void InternalSaveToFile(string path, IPersister persister, RestoreState state)
    {
        persister.SaveObject(state, path+"." + Consts.FileExtension_TickerModes);
        RestoreAbleService.SaveRestoreAblesToFile(Parameters.Where(p => p != null),path + "/" + Consts.FolderName_Parameters, persister);
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
