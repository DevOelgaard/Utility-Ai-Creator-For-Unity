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

    internal static AiLog GetDebug(Uai uai, int tick)
    {
        var result = new AiLog();
        result = SetBasics(result, uai, tick) as AiLog;
        result.Buckets = new List<BucketLog>();
        foreach (var bucket in uai.Buckets.Values)
        {
            result.Buckets.Add(BucketLog.GetDebug(bucket, tick));
        }

        result.BucketSelector = UCSLog.GetDebug(uai.CurrentBucketSelector, tick);
        result.DecisionSelector = UCSLog.GetDebug(uai.CurrentDecisionSelector, tick);
        return result;
    }
}