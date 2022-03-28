using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[Serializable]
public class AiContext
{
    public IAgent Agent;
    internal AgentAction CurrentAction;
    private Dictionary<string, object> contextStringKey = new Dictionary<string, object>();
    private Dictionary<AiContextKey, object> contextEnumKey = new Dictionary<AiContextKey, object>();
    internal IUtilityScorer UtilityScorer = new USAverageScorer();
    internal List<AgentAction> LastActions = new List<AgentAction>();
    internal Decision LastSelectedDecision;
    internal Decision CurrentEvalutedDecision;
    internal Bucket LastSelectedBucket;
    internal Bucket CurrentEvaluatedBucket;
    internal TickMetaData TickMetaData;
         
    public AiContext()
    {
    }

    /// <summary>
    /// This is slower but more versatile. Consider using GetContext(AiContextKey) instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetContext<T>(string key)
    {
        if (contextStringKey.ContainsKey(key))
        {
            return (T)contextStringKey[key];
        } else
        {
            return default(T);
        }
    }

    /// <summary>
    /// This is slower but more versatile. Consider using SetContext(AiContextKey) instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public void SetContext(string key, object value)
    {
        if (!contextStringKey.ContainsKey(key))
        {
            contextStringKey.Add(key, value);
        } else
        {
            contextStringKey[key] = value;
        }
    }

    /// <summary>
    /// This is slower but more versatile. Consider using RemoveContext(AiContextKey) instead.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public void RemoveContext(string key)
    {
        if (contextStringKey.ContainsKey(key))
        {
            contextStringKey.Remove(key);
        }
    }

    /// <summary>
    /// Use this if you have defined the needed Enum in AiContextKey otherwise use the string version
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public T GetContext<T>(AiContextKey key)
    {
        if (contextEnumKey.ContainsKey(key))
        {
            return (T)contextEnumKey[key];
        }
        else
        {
            return default;
        }
    }

    /// <summary>
    /// Use this if you have defined the needed Enum in AiContextKey otherwise use the string version
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public void SetContext(AiContextKey key, object value)
    {
        if (!contextEnumKey.ContainsKey(key))
        {
            contextEnumKey.Add(key, value);
        }
        else
        {
            contextEnumKey[key] = value;
        }
    }

    /// <summary>
    /// Use this if you have defined the needed Enum in AiContextKey otherwise use the string version
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public void RemoveContext(AiContextKey key)
    {
        if (contextEnumKey.ContainsKey(key))
        {
            contextEnumKey.Remove(key);
        }
    }

}
