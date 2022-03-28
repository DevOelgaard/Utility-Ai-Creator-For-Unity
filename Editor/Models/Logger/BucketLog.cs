using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

internal class BucketLog: AiObjectLog
{
    internal List<ConsiderationLog> Considerations;
    internal List<DecisionLog> Decisions;
    internal float Score;
    internal float Weight;

    internal static BucketLog GetDebug(Bucket bucket, int tick)
    {
        var result = new BucketLog();
        result = SetBasics(result, bucket, tick) as BucketLog;
        result.Score = bucket.ScoreModels.First().Value;
        result.Weight = Convert.ToSingle(bucket.Weight.Value);

        result.Considerations = new List<ConsiderationLog>();
        foreach (var consideration in bucket.Considerations.Values)
        {
            result.Considerations.Add(ConsiderationLog.GetDebug(consideration, tick));
        }

        result.Decisions = new List<DecisionLog>();
        foreach(var decision in bucket.Decisions.Values)
        {
            result.Decisions.Add(DecisionLog.GetDebug(decision, tick));
        }

        return result;
    }
}