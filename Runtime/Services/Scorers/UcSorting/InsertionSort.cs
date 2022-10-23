using System.Collections.Generic;

internal class InsertionSort
{
        internal static List<Bucket> SortHighestFirst(List<Bucket> list)
        {
                for (var i = 1; i < list.Count; i++)
                {
                    var key = list[i];
                    var flag = 0;
                    for (var j = i-1; j >= 0 && flag != 1; j++)
                    {
                        if (key.Utility > list[j].Utility)
                        {
                            list[j + 1] = list[j];
                            j--;
                            list[j + 1] = key;
                        }
                        else
                        {
                            flag = 1;
                        }
                    }
                }
                return list;
        }
}