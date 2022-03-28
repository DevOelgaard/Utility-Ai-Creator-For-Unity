using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class RestoreAbleCollection: RestoreAble
{
    internal List<RestoreAble> Models = new List<RestoreAble>();
    public Type Type { get; private set; }
    public RestoreAbleCollection()
    {
    }

    public RestoreAbleCollection(List<RestoreAble> states, Type type)
    {
        this.Models = states;
        this.Type = type;
    }

    internal override RestoreState GetState()
    {
        return new RestoreAbleCollectionState(Models, Type, this);
    }


    protected override void RestoreInternal(RestoreState state, bool restoreDebug = false)
    {
        var s = state as RestoreAbleCollectionState;
        Models = new List<RestoreAble>();
        s.States.ForEach(e =>
        {
            Models.Add(Restore<RestoreAble>(e, restoreDebug));
        });
        Type = Type.GetType(s.TypeString);
    }

    internal override void SaveToFile(string path, IPersister persister)
    {
        var state = GetState() as RestoreAbleCollectionState;
        persister.SaveObject(state, path);
    }
}

public class RestoreAbleCollectionState: RestoreState
{
    public List<RestoreState> States;
    public string TypeString;

    public RestoreAbleCollectionState()
    {
    }

    internal RestoreAbleCollectionState(List<RestoreAble> restoreAbles, Type typeString, RestoreAbleCollection o) : base(o)
    {
        States = new List<RestoreState>();
        restoreAbles.ForEach(e =>
        {
            States.Add(e.GetState());
        });
        TypeString = typeString.ToString();
    }
}
