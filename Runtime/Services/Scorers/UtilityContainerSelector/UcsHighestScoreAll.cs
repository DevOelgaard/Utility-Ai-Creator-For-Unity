// using System.Collections.Generic;
// using System.Linq;
//
// public class UcsHighestScoreAll: UtilityContainerSelector
// {
//     private readonly string name = Consts.UCS_HighestScore_Name;
//     private readonly string description = Consts.UCS_HighestScore_Description;
//     public override string GetDescription()
//     {
//         return description;
//     }
//
//     public override string GetName()
//     {
//         return name;
//     }
//     public override Bucket GetBestUtilityContainer(List<Bucket> containers, IAiContext context)
//     {
//         UtilityContainer bestBucket = null;
//         var highestUtility = float.MinValue;
//
//         foreach (var bucket in containers)
//         {
//             if (bucket.Weight.Value < highestUtility)
//             {
//                 continue;
//             }
//             
//             context.CurrentEvaluatedBucket = bucket;
//             var currentUtility = bucket.GetUtility(context);
//             if (currentUtility > highestUtility)
//             {
//                 bestBucket = bucket;
//                 highestUtility = currentUtility;
//             }
//         }
//         // Evaluate first bucket
//
//         // If next bucket.Weight < currentBestBucket.score - continue
//
//         // else 
//         // Evaluate bucket and check score against currentBestBucket
//
//
//     }
//
//     public override Decision GetBestUtilityContainer(List<Decision> containers, IAiContext context)
//     {
//         throw new System.NotImplementedException();
//     }
// }