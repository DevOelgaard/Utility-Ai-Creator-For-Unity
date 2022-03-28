using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniRxExtension;


namespace UniRxExtension
{
    internal class ReactiveListSorted<T,TKey> : ReactiveList<T>
    {
        private bool descending;
        private Func<T, TKey> sortFunction;
        public ReactiveListSorted(Func<T, TKey> sortFunction, bool descending): base()
        {
            this.descending = descending;
        }

        public override void Add(T element)
        {
            List.Add(element);

            if (descending)
            {
                List.OrderByDescending(sortFunction);
            } else
            {
                List.OrderBy(sortFunction);
            }

            onValueChanged.OnNext(List);
        }
    }
}