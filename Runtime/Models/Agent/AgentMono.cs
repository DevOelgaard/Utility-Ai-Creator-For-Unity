using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class AgentMono : MonoBehaviour, IAgent
{
    [SerializeField]
    [InspectorName("Settings")]
    private AgentModel settings = new AgentModel();
    public AgentModel Model => settings;
    public string TypeIdentifier => GetType().FullName;

    [HideInInspector]
    public string DefaultAiName = "";

    private DecisionScoreEvaluator decisionScoreEvaluator;
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

    void Start()
    {
        Model.Name = SetAgentName();
        AgentManager.Instance.Register(this);
        var ai = UASTemplateService.Instance.GetAiByName(DefaultAiName,true);
        SetAi(ai);
        decisionScoreEvaluator = new DecisionScoreEvaluator();
    }

    public void SetAi(Ai model)
    {
        Ai = model;
    }

    void OnDestroy()
    {
        AgentManager.Instance?.Unregister(this);
    }

    /// <summary>
    /// Returns the desired AiAgent name, which is displayd in the UAS Tools
    /// By default set as the name of the attached MonoBehaviour
    /// </summary>
    /// <returns></returns>
    protected virtual string SetAgentName()
    {
        return gameObject.name;
    }

    public void Tick(TickMetaData metaData)
    {
        Ai.Context.TickMetaData = metaData;
        Model.LastTickMetaData = metaData;
        Model.LastTickTime = Time.time;
        Model.LastTickFrame = Time.frameCount;

        var actions = decisionScoreEvaluator.NextActions(Ai.Buckets.Values, Ai.Context, Ai);
        var oldActions = Ai.Context.LastActions;
        foreach(var action in actions)
        {
            if (oldActions.Contains(action))
            {
                action.OnGoing(Ai.Context);
                oldActions.Remove(action);
            } else
            {
                action.OnStart(Ai.Context);
            }
        }

        foreach(var action in oldActions)
        {
            action.OnEnd(Ai.Context);
        }

        Ai.Context.LastActions = actions;
    }

    public bool CanAutoTick()
    {
        if (Model.AutoTick == false) return false;
        if (Time.time - Model.LastTickTime < Model.MsBetweenTicks/1000) return false;
        if (Time.frameCount - Model.LastTickFrame < Model.FramesBetweenTicks) return false;
        return true;
    }
}
