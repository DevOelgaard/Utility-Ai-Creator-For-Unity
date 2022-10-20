using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class DseHighestBuckets: DecisionScoreEvaluator
{
    private const string Name = Consts.Name_HighestBucketDSE;
    private const string Description = Consts.Description_HighestBucketDSE;

    public DseHighestBuckets()
    {
    }

    public override string GetDescription()
    {
        return Description;
    }

    public override string GetName()
    {
        return Name;
    }

    public override List<AgentAction> NextActions(List<Decision> decisions, Uai uai)
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
            SetContextValuesDecision(uai.UaiContext,bestDecision);

            return bestDecision.AgentActions.Values;
        }
    }

    public override List<AgentAction> NextActions(List<Bucket> buckets, Uai uai)
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
                SetContextValuesBucket(uai.UaiContext,bestBucket);
                return nextActions;
            }
        }
    }
}
