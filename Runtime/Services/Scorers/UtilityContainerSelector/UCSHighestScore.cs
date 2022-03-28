using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UCSHighestScore : UtilityContainerSelector
{
    private string name = Consts.UCS_HighestScore_Name;
    private string description = Consts.UCS_HighestScore_Description;

    public override string GetDescription()
    {
        return description;
    }

    public override string GetName()
    {
        return name;
    }

    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>();
    }

    public override Bucket GetBestUtilityContainer(List<Bucket> buckets, AiContext context)
    {
        UtilityContainer bestContainer = null;
        foreach(Bucket bucket in buckets)
        {
            var weight = Convert.ToSingle(bucket.Weight.Value);

            // Only evaluate if the bucket has a chance of winning
            if (bestContainer != null && weight < bestContainer.LastCalculatedUtility)
            {
                continue;
            }
            context.CurrentEvaluatedBucket = bucket;
            bestContainer = CheckBestContainer(bucket, context, bestContainer);
        }
        return bestContainer as Bucket;
    }

    public override Decision GetBestUtilityContainer(List<Decision> decisions, AiContext context)
    {
        UtilityContainer bestContainer = null;
        foreach (var decision in decisions)
        {
            context.CurrentEvalutedDecision = decision;
            bestContainer = CheckBestContainer(decision, context, bestContainer);
        }
        return bestContainer as Decision;
    }

    private UtilityContainer CheckBestContainer(UtilityContainer container, AiContext context, UtilityContainer bestContainer = null)
    {
        var utility = container.GetUtility(context);
        
        if (utility <= 0)
        {
            return bestContainer;
        }

        if (bestContainer == null)
        {
            return container;
        } else
        {
            if(utility > bestContainer.LastCalculatedUtility)
            {
                return container;
            }
        }
        return bestContainer;
    }
}
