using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class AiLog: AiObjectLog
{
    internal List<BucketLog> Buckets;
    internal UCSLog BucketSelector;
    internal UCSLog DecisionSelector;

    internal static AiLog GetDebug(Ai ai, int tick)
    {
        var result = new AiLog();
        result = SetBasics(result, ai, tick) as AiLog;
        result.Buckets = new List<BucketLog>();
        foreach (var bucket in ai.Buckets.Values)
        {
            result.Buckets.Add(BucketLog.GetDebug(bucket, tick));
        }

        result.BucketSelector = UCSLog.GetDebug(ai.CurrentBucketSelector, tick);
        result.DecisionSelector = UCSLog.GetDebug(ai.CurrentDecisionSelector, tick);
        return result;
    }
}