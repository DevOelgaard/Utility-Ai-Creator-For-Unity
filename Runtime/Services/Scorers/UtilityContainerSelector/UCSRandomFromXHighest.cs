using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class UCSRandomFromXHighest : UtilityContainerSelector
{
    private int NumberOfItemsToEvaluate
    {
        get
        {
            var val = Convert.ToInt32(Parameters[0].Value);
            if (val > 0)
            {
                return val;
            } else
            {
                return int.MaxValue;
            }       
        }
    }
    private bool PercentageChance => (bool)Parameters[1].Value;

    private float MaxDeviationFromHighest => Convert.ToSingle(Parameters[2].Value);
    protected override List<Parameter> GetParameters()
    {
        return new List<Parameter>()
        {
            new Parameter("Number Of Items", 1),
            new Parameter("Percentage chance", true),
            new Parameter("Max deviation from highest", 1f)
        };
    }
    public override Bucket GetBestUtilityContainer(List<Bucket> buckets, AiContext context)
    {
        buckets = buckets
            .OrderByDescending(b => b.Weight)
            .ToList();

        var result = new List<UtilityContainer>();

        foreach(Bucket bucket in buckets)
        {
            var lowestValidValue = GetLowestValidValue(result);
            if (Convert.ToSingle(bucket.Weight.Value) < lowestValidValue)
            {
                continue;
            } else
            {
                context.CurrentEvaluatedBucket = bucket;
                bucket.GetUtility(context);
                result = UpdateList(result, bucket);
            }
        }
        return (Bucket)GetRandomContainer(result);

    }

    public override Decision GetBestUtilityContainer(List<Decision> decisions, AiContext context)
    {
        var result = new List<UtilityContainer>();
        foreach (Decision decision in decisions)
        {
            context.CurrentEvalutedDecision = decision;
            decision.GetUtility(context);
            result = UpdateList(result, decision);
        }
        return (Decision)GetRandomContainer(result);
    }

    public override string GetDescription()
    {
        return Consts.UCS_RandomXHighest_Description;
    }

    public override string GetName()
    {
        return Consts.UCS_RandomXHighest_Name;
    }

    private List<UtilityContainer> UpdateList(List<UtilityContainer> list, UtilityContainer container)
    {
        if (container.LastCalculatedUtility <= 0 )
        {
            return list;
        }

        // Return if score is to far from highest score.
        var highestValid = list.FirstOrDefault();
        
        if (highestValid != null)
        {
            if (list.IndexOf(highestValid) > NumberOfItemsToEvaluate - 1)
            {
                highestValid = list[NumberOfItemsToEvaluate - 1];
            }
            var minimumAllowedScore = highestValid.LastCalculatedUtility - MaxDeviationFromHighest;
            if (container.LastCalculatedUtility < minimumAllowedScore)
            {
                return list;
            }
        }

        var evaluateIndex = NumberOfItemsToEvaluate - 1;

        if (list.Count < NumberOfItemsToEvaluate)
        {
            list.Add(container);
        } 
        else if (container.LastCalculatedUtility < list[evaluateIndex].LastCalculatedUtility)
        {
            return list;
        } 
        else if (container.LastCalculatedUtility < list[evaluateIndex].LastCalculatedUtility)
        {
            var rand = UnityEngine.Random.Range(0, 2);
            if (rand == 0) // Swapping two equally scored containers at random
            {
                list[evaluateIndex] = container;
            }
        } 
        else
        {
            list.Add(container);
        }
        list = list
            .OrderByDescending(uc => uc.LastCalculatedUtility)
            .ToList();
        return list;
    }

    private UtilityContainer GetRandomContainer(List<UtilityContainer> list)
    {
        if (list.Count == 0)
        {
            throw new Exception("No items to chose from, list.Count must be > 0");
        }
        var numberOfItems = NumberOfItemsToEvaluate < list.Count ? NumberOfItemsToEvaluate : list.Count;

        if (PercentageChance)
        {
            var sum = list
                .Take(numberOfItems)
                .Where(uc => uc != null)
                .Sum(uc => uc.LastCalculatedUtility);
            

            var resultNumber = UnityEngine.Random.Range(0, sum);
            for(var i = 0; i < numberOfItems; i++)
            {
                resultNumber -= list[i].LastCalculatedUtility;
                if(resultNumber <= 0)
                {
                    return list[i];
                }
            }
        }
        else
        {
            var rand = UnityEngine.Random.Range(0, numberOfItems);
            return list[rand];
        }

        throw new Exception("Something should have been chosen at this point");
    }
    private float GetLowestValidValue(List<UtilityContainer> list)
    {
        if (list.Count() == 0)
        {
            return default;
        }
        else if (list.Count() < NumberOfItemsToEvaluate)
        {
            return list.Last().LastCalculatedUtility;
        }
        else
        {
            return list[NumberOfItemsToEvaluate - 1].LastCalculatedUtility;
        }
    }
}