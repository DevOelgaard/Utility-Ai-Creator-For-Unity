using System.Collections.Generic;

public interface IAiContext
{
    IAgent Agent { get; set; }
    IUtilityScorer UtilityScorer { get; set; }
    List<AgentAction> LastActions { get; set; }

    /// <summary>
    /// If called from an AgentAction this will be the object selected this tick.
    /// If called from another AiObject this will be the object selected last tick.
    /// </summary>
    Decision LastSelectedDecision { get; set; }

    Decision CurrentEvaluatedDecision { get; set; }

    /// <summary>
    /// If called from an AgentAction this will be the object selected this tick.
    /// If called from another AiObject this will be the object selected last tick.
    /// </summary>public Bucket LastSelectedBucket { get; internal set; }
    Bucket LastSelectedBucket { get; set; }

    Bucket CurrentEvaluatedBucket { get; set; }
    TickMetaData TickMetaData { get; set; }

    /// <summary>
    /// Get context created by the requester or any of its parents.
    /// </summary>
    /// <param name="key">The key to the data stored in the context.</param>
    /// <param name="requester">The requester of the data. If null there is only checked for global context.</param>
    /// <returns></returns>
    T GetContext<T>(object key, AiObjectModel requester = null);

    /// <summary>
    /// Sets context data destructively. Data is available to the owner and all of its children.
    /// </summary>
    /// <param name="key">The key for retrieving data</param>
    /// <param name="value">The data to store</param>
    /// <param name="owner">The owner of the data. If null the data is stored locally</param>
    /// <returns></returns>
    void SetContext(object key, object value, AiObjectModel owner = null);

    /// <summary>
    /// Removes context data at the given key.
    /// </summary>
    /// <param name="key">The key to identify the data.</param>
    /// <param name="owner">The owner of the data. If null the data removed from global context</param>
    /// <returns></returns>
    void RemoveContext(object key, AiObjectModel owner = null);
}