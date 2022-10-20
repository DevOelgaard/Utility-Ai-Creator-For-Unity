using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class DecisionScoreEvaluator: IIdentifier
{
    public abstract string GetName();

    public abstract string GetDescription();
    public abstract List<AgentAction> NextActions(List<Decision> decisions, Uai uai);
    public abstract List<AgentAction> NextActions(List<Bucket> buckets, Uai uai);
    

    protected void SetContextValuesBucket(IAiContext context, Bucket bestBucket)
    {
        context.LastSelectedBucket = bestBucket;
        bestBucket.MetaData.LastTickSelected = context.TickMetaData.TickCount;
    }

    protected void SetContextValuesDecision(IAiContext context,Decision bestDecision)
    {
        context.LastSelectedDecision = bestDecision;
        context.LastSelectedDecision.LastSelectedTickMetaData = context.TickMetaData;
        bestDecision.MetaData.LastTickSelected = context.TickMetaData.TickCount;
    }
}
