using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public abstract class AgentScript : IAgent
{
    private AgentModel model = new AgentModel();
    public AgentModel Model => model;

    public string TypeIdentifier => GetType().FullName;

    [SerializeField]
    protected string DefaultAiName => GetDefaultAiName();

    private Ai ai;
    public Ai Ai
    {
        get => ai;
        set
        {
            ai = value;
            ai.Context.Agent = this;
        }
    }

    public AgentScript()
    {
        model.Name = SetAgentName();
        var ai = UASTemplateService.Instance.GetAiByName(DefaultAiName);
        SetAi(ai);
        AgentManager.Instance.Register(this);
    }

    ~AgentScript()
    {
        AgentManager.Instance?.Unregister(this);
    }

    /// <summary>
    /// Returns the desired AiAgent name, which is displayd in the UAS Tools.
    /// By default set as class name
    /// </summary>
    /// <returns></returns>
    protected virtual string SetAgentName()
    {
        return TypeIdentifier;
    }

    protected virtual string GetDefaultAiName()
    {
        return "";
    }

    public void Tick(TickMetaData metaData)
    {
        throw new NotImplementedException();
        //Ai.Context.SetContext(AiContextKey.TickMetaData, metaData);
        //var actions = decisionScoreEvaluator.NextActions(Ai.Buckets.Values, Ai.Context);
        //foreach (var action in actions)
        //{
        //    action.OnStart(Ai.Context);
        //}
    }

    public void SetAi(Ai model)
    {
        Ai = model;
    }

    public bool CanAutoTick()
    {
        throw new NotImplementedException();
    }
}