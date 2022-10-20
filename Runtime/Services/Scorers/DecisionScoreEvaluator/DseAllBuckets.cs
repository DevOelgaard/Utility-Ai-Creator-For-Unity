using System.Collections.Generic;

public class DseAllBuckets: DecisionScoreEvaluator
{
    private const string Name = Consts.Name_AllBucketsDSE;
    private const string Description = Consts.Description_AllBucketsDSE;
    public override string GetDescription()
    {
        return Description;
    }

    public override string GetName()
    {
        return Name;
    }
    
    public override List<AgentAction> NextActions(List<Bucket> buckets, Uai uai)
    {
        var context = uai.UaiContext;
        foreach (var bucket in buckets)
        {
            context.CurrentEvaluatedBucket = bucket;
            bucket.GetUtility(context);
        }

        var sorted = InsertionSort.SortHighestFirst(new List<Bucket>(buckets));

        Decision bestDecision = null;
        var bestUtility = float.MinValue;
        foreach (var bucket in sorted)
        {
            // If weight is to low, the decisions can't score high enough
            if (bucket.Weight.Value < bestUtility) continue;
            foreach (var decision in bucket.Decisions.Values)
            {
                var utility = decision.GetUtility(context);
                if (utility > bestUtility)
                {
                    bestUtility = utility;
                    bestDecision = decision;
                }
            }
        }

        return bestDecision == null ? 
            new List<AgentAction>() : 
            bestDecision.AgentActions.Values;
    }

    public override List<AgentAction> NextActions(List<Decision> decisions, Uai uai)
    {
        throw new System.NotImplementedException();
    }
}