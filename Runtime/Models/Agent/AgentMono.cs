using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using Debug = UnityEngine.Debug;


public class AgentMono : MonoBehaviour, IAgent
{
    [SerializeField]
    [InspectorName("Settings")]
    private AgentModel model = new AgentModel();

    private Stopwatch stopwatch = new Stopwatch();
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
    

    protected virtual void Start()
    {
        Debug.Log("Called");
        Model.Name = SetAgentName();
        AgentManager.Instance.Register(this);
        var aiByName = PlayAbleAiService.Instance.GetAiByName(defaultAiName);
        SetAi(aiByName);
        decisionScoreEvaluator = new DecisionScoreEvaluator();
    }

    public void SetAi(Uai newUai)
    {
        if (newUai == null)
        {
            DebugService.LogWarning("Setting Ai of agent: " + name +" to null", this);
            throw new NullReferenceException();
        }
        DebugService.Log("Setting Ai of agent: " + newUai.Name, this);
        Uai = newUai;
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

    public void ActivateNextAction(TickMetaData metaData)
    {
        stopwatch.Restart();
        if (Uai == null)
        {
            var aiByName = PlayAbleAiService.Instance.GetAiByName(defaultAiName);
            SetAi(aiByName);
        }
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

        metaData.ExecutionTimeInTicks = stopwatch.ElapsedTicks;
    }

    public bool CanAutoTick()
    {
        if (Model.AutoTick == false) return false;
        if (Time.time - Model.LastTickTime < Model.MsBetweenTicks/1000f) return false;
        return Time.frameCount - Model.LastTickFrame >= Model.FramesBetweenTicks;
    }
}
