using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DecisionScoreEvaluator: IDecisionScoreEvaluator
{
    private readonly string name = Consts.Name_DefaultDSE;
    private readonly string description = Consts.Description_DefaultDSE; 

    public DecisionScoreEvaluator()
    {
    }

    public string GetDescription()
    {
        return description;
    }

    public string GetName()
    {
        return name;
    }

    public List<AgentAction> NextActions(List<Decision> decisions, Uai uai)
    {
        if (decisions.Count == 0)
        {
            return new List<AgentAction>();
        } else 
        {
            var bestDecision = uai.CurrentDecisionSelector.GetBestUtilityContainer(decisions, uai.UaiContext);
            if (bestDecision == null || bestDecision.LastCalculatedUtility <= 0)
            {
                //Debug.LogWarning("No valid decision. Add a \"fall back\" decision (Ie. Idle), which always scores >0");
                return new List<AgentAction>();
                //throw new Exception("No valid decision. Add a \"fall back\" decision (Ie. Idle), which always scores >0");
            }
            uai.UaiContext.LastSelectedDecision = bestDecision;
            uai.UaiContext.LastSelectedDecision.LastSelectedTickMetaData = uai.UaiContext.TickMetaData;
            bestDecision.MetaData.LastTickSelected = uai.UaiContext.TickMetaData.TickCount;
            return bestDecision.AgentActions.Values;
        }
    }

    public List<AgentAction> NextActions(List<Bucket> buckets, Uai uai)
    {
        if (buckets.Count == 0)
        {
            return new List<AgentAction>();
        }
        else
        {
            var bestBucket = uai.CurrentBucketSelector.GetBestUtilityContainer(buckets, uai.UaiContext);
            if (bestBucket == null)
            {
                return new List<AgentAction>();
            }
            var nextActions = NextActions(bestBucket.Decisions.Values, uai);
            if (nextActions == null || nextActions.Count == 0)
            {
                return NextActions(buckets.Where(bucket => bucket != bestBucket).ToList(), uai);
            }
            else
            {
                uai.UaiContext.LastSelectedBucket = bestBucket;
                bestBucket.MetaData.LastTickSelected = uai.UaiContext.TickMetaData.TickCount;
                return nextActions;
            }
        }
    }
}
