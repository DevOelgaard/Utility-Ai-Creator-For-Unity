using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;


public class AgentMono : MonoBehaviour, IAgent
{
    [SerializeField]
    [InspectorName("Settings")]
    private AgentModel model = new AgentModel();
    public AgentModel Model => model;
    public string TypeIdentifier => GetType().FullName;

    [HideInInspector]
    public string defaultAiName = "";

    private DecisionScoreEvaluator decisionScoreEvaluator;
    private Uai uai;
    public Uai Uai
    {
        get => uai;
        set
        {
            uai = value;
            uai.UaiContext.Agent = this;
        }
    }
    

    void Start()
    {
        Model.Name = SetAgentName();
        AgentManager.Instance.Register(this);
        var aiByName = PlayAbleAiService.Instance.GetAiByName(defaultAiName);
        SetAi(aiByName);
        decisionScoreEvaluator = new DecisionScoreEvaluator();
    }

    public void SetAi(Uai model)
    {
        if (model == null)
        {
            DebugService.LogWarning("Setting Ai of agent: " + name +" to null", this);
            throw new NullReferenceException();
        }
        DebugService.Log("Setting Ai of agent: " + model.Name, this);
        Uai = model;
    }

    void OnDestroy()
    {
        AgentManager.Instance?.Unregister(this);
    }

    /// <summary>
    /// Returns the desired AiAgent name, which is displayed in the UAS Tools
    /// By default set as the name of the attached MonoBehaviour
    /// </summary>
    /// <returns></returns>
    protected virtual string SetAgentName()
    {
        return gameObject.name;
    }

    public void Tick(TickMetaData metaData)
    {
        Uai.UaiContext.TickMetaData = metaData;
        Model.LastTickMetaData = metaData;
        Model.LastTickTime = Time.time;
        Model.LastTickFrame = Time.frameCount;

        var actions = decisionScoreEvaluator.NextActions(Uai.Buckets.Values, Uai);
        var oldActions = Uai.UaiContext.LastActions;
        foreach(var action in actions)
        {
            if (oldActions.Contains(action))
            {
                action.OnGoing(Uai.UaiContext);
                oldActions.Remove(action);
            } else
            {
                action.OnStart(Uai.UaiContext);
            }
        }

        foreach(var action in oldActions)
        {
            action.OnEnd(Uai.UaiContext);
        }

        Uai.UaiContext.LastActions = actions;
    }

    public bool CanAutoTick()
    {
        if (Model.AutoTick == false) return false;
        if (Time.time - Model.LastTickTime < Model.MsBetweenTicks/1000f) return false;
        return Time.frameCount - Model.LastTickFrame >= Model.FramesBetweenTicks;
    }
}
