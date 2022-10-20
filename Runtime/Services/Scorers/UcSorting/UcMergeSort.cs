// using System.Collections.Generic;
//
// public class UcMergeSort
// {
//         internal static List<UtilityContainer> SortUtilityContainers(List<UtilityContainer> containers, int left, int right)
//         {
//                 if (left < right)
//                 {
//                         var middle = left + (right - left) / 2;
//                         SortUtilityContainers(containers, left, middle);
//                         SortUtilityContainers(containers, middle + 1, right);
//
//                         MergeUtilityContainers(containers, left, middle, right);
//                 }
//
//                 return containers;
//         }
//
//         private static void MergeUtilityContainers(List<UtilityContainer> containers, int left, int middle, int right)
//         {
//                 var leftArrayLength = middle - left + 1;
//                 var rightArrayLength = right - middle;
//                 // var left
//         }
// }