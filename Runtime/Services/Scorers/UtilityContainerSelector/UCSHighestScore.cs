using System;
using System.Collections.Generic;

public class UCSHighestScore : UtilityContainerSelector
{
    private readonly string name = Consts.UCS_HighestScore_Name;
    private readonly string description = Consts.UCS_HighestScore_Description;

    public override string GetDescription()
    {
        return description;
    }

    public override string GetName()
    {
        return name;
    }

    public override Bucket GetBestUtilityContainer(List<Bucket> buckets, IAiContext context)
    {
        UtilityContainer bestContainer = null;
        foreach(Bucket currentlyEvaluatedBucket in buckets)
        {
            context.CurrentEvaluatedBucket = currentlyEvaluatedBucket;
            bestContainer = CheckBestContainer(currentlyEvaluatedBucket, context, bestContainer);
        }
        return bestContainer as Bucket;
    }

    public override Decision GetBestUtilityContainer(List<Decision> decisions, IAiContext context)
    {
        UtilityContainer bestContainer = null;
        foreach (var currentlyEvaluatedDecision in decisions)
        {
            context.CurrentEvaluatedDecision = currentlyEvaluatedDecision;
            bestContainer = CheckBestContainer(currentlyEvaluatedDecision, context, bestContainer);
        }
        return bestContainer as Decision;
    }

    private UtilityContainer CheckBestContainer(UtilityContainer currentlyEvaluatedContainer, IAiContext context, UtilityContainer bestContainer = null)
    {
        var utility = currentlyEvaluatedContainer.GetUtility(context);

        if (utility <= 0)
        {
            return null;
        }

        if (bestContainer == null)
        {
            return currentlyEvaluatedContainer;
        } else
        {
            if(utility > bestContainer.LastCalculatedUtility)
            {
                return currentlyEvaluatedContainer;
            }
        }
        return bestContainer;
    }
}
