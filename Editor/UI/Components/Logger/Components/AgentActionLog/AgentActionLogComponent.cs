using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AgentActionLogComponent : AiObjectLogComponent
{
    private LogComponentPool<ParameterLogComponent> parameterPool;
    public AgentActionLogComponent() : base()
    {
        parameterPool = new LogComponentPool<ParameterLogComponent>(Body, false,"Parameters",1);
    }

    protected override void UpdateUiInternal(AiObjectLog aiObjectDebug)
    {
        var a = aiObjectDebug as AgentActionLog;
        var logModels = new List<ILogModel>();
        NameLabel.text = a.Name;
        foreach (var p in a.Parameters)
        {
            logModels.Add(p);
        }
        parameterPool.Display(logModels);
    }

    internal override void Hide()
    {
        base.Hide();
        parameterPool.Hide();
    }
}
