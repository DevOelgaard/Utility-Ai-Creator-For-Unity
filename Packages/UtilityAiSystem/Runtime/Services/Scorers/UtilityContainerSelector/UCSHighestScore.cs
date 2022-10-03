using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

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
        foreach(Bucket bucket in buckets)
        {
            //
            var weight = Convert.ToSingle(bucket.Weight.Value);

            context.CurrentEvaluatedBucket = bucket;
            bestContainer = CheckBestContainer(bucket, context, bestContainer);
        }
        return bestContainer as Bucket;
    }

    public override Decision GetBestUtilityContainer(List<Decision> decisions, IAiContext context)
    {
        UtilityContainer bestContainer = null;
        foreach (var decision in decisions)
        {
            context.CurrentEvaluatedDecision = decision;
            bestContainer = CheckBestContainer(decision, context, bestContainer);
        }
        return bestContainer as Decision;
    }

    private UtilityContainer CheckBestContainer(UtilityContainer container, IAiContext context, UtilityContainer bestContainer = null)
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
