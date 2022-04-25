﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class AiContext
{
    public IAgent Agent;
    private Dictionary<object, object> context = new Dictionary<object, object>();
    public IUtilityScorer UtilityScorer = new UsAverageScorer();
    public List<AgentAction> LastActions = new List<AgentAction>();
    /// <summary>
    /// If called from an AgentAction this will be the object selected this tick.
    /// If called from another AiObject this will be the object selected last tick.
    /// </summary>
    public Decision LastSelectedDecision { get; internal set; }
    public Decision CurrentEvaluatedDecision { get; internal set; }
    
    /// <summary>
    /// If called from an AgentAction this will be the object selected this tick.
    /// If called from another AiObject this will be the object selected last tick.
    /// </summary>public Bucket LastSelectedBucket { get; internal set; }
    public Bucket LastSelectedBucket { get; internal set; }
    public Bucket CurrentEvaluatedBucket { get; internal set; }
    public TickMetaData TickMetaData { get; internal set; }
    private const int IterationMax = 20;

    public AiContext()
    {
    }

    /// <summary>
    /// Get context created by the requester or any of its parents.
    /// </summary>
    /// <param name="key">The key to the data stored in the context.</param>
    /// <param name="requester">The requester of the data. If null there is only checked for global context.</param>
    /// <returns></returns>
    public T GetContext<T>(object key, AiObjectModel requester = null)
    {
        var numberOfIterations = 0;
        while (true)
        {
            //Base case
            if (requester == null)
            {
                // If global value is set
                if (context.ContainsKey(key))
                {
                    return (T) context[key];
                }
                else
                {
                    return default;
                }
            }

            var requesterKey = GetKey(key, requester);
            if (context.ContainsKey(requesterKey))
            {
                return (T) context[requesterKey];
            }

            requester = requester.ca.Parent;

            numberOfIterations++;
            if (numberOfIterations > IterationMax)
            {
                DebugService.LogWarning("Failed to find AiContext at key: " + key + " and failed to break out of loop.",this);
            }
        }
    }

    /// <summary>
    /// Sets context data destructively. Data is available to the owner and all of its children.
    /// </summary>
    /// <param name="key">The key for retrieving data</param>
    /// <param name="value">The data to store</param>
    /// <param name="owner">The owner of the data. If null the data is stored locally</param>
    /// <returns></returns>
    public void SetContext(object key, object value, AiObjectModel owner = null)
    {
        var requesterKey = GetKey(key, owner);

        if (!context.ContainsKey(requesterKey))
        {
            context.Add(requesterKey, value);
        } else
        {
            context[requesterKey] = value;
        }
    }

    /// <summary>
    /// Removes context data at the given key.
    /// </summary>
    /// <param name="key">The key to identify the data.</param>
    /// <param name="owner">The owner of the data. If null the data removed from global context</param>
    /// <returns></returns>
    public void RemoveContext(object key, AiObjectModel owner = null)
    {
        var ownerKey = GetKey(key, owner);
        if (context.ContainsKey(ownerKey))
        {
            context.Remove(key);
        }
    }
    
    private string GetKey(object key, AiObjectModel requester)
    {
        return key + requester?.ca.Address;
    }
}
