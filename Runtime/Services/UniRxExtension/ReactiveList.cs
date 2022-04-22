using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace UniRxExtension
{
    [Serializable]
    public class ReactiveList<T>
    {
        protected List<T> List;
        public IObservable<List<T>> OnValueChanged => onValueChanged;
        protected Subject<List<T>> onValueChanged;

        public ReactiveList()
        {
            List = new List<T>();
            onValueChanged = new Subject<List<T>>();
        }

        public ReactiveList(List<T> list){
            this.List = list;
            onValueChanged = new Subject<List<T>>();
        }


        public List<T> Values => CopyList();

        private List<T> CopyList()
        {
            return new List<T>(List);
        }

        public virtual void Add(T element)
        {
            List.Add(element);
            onValueChanged.OnNext(Values);
        }

        public virtual void Add(IEnumerable<T> elements)
        {
            
            var elementsAsList = elements.ToList();
            if (!elementsAsList.Any())
            {
                DebugService.Log("Returning no elements to add", nameof(ReactiveList<T>));
            }
            DebugService.Log("Adding " + elementsAsList.Count() + " elements", nameof(ReactiveList<T>));
            foreach(var e in elementsAsList)
            {
                List.Add(e);
            }
            onValueChanged.OnNext(Values);
        }

        public virtual void Remove(T element)
        {
            List.Remove(element);
            onValueChanged.OnNext(Values);
        }

        public virtual void ChangeAll(List<T> elements)
        {
            List = elements;
            onValueChanged.OnNext(Values);
        }

        public void Clear()
        {
            List = new List<T>();
            onValueChanged.OnNext(Values);
        }

        public void ClearNoNotify()
        {
            List = new List<T>();
        }

        public void IncreaIndex(T element)
        {
            var index = List.IndexOf(element);
            if (index >= List.Count-1) return;
            var itemToReplace = List[index+1];
            List[index + 1] = element;
            List[index] = itemToReplace;
            onValueChanged.OnNext(List);
        }

        public void DecreaseIndex(T element)
        {
            var index = List.IndexOf(element);
            if (index <= 0) return;
            var itemToReplace = List[index - 1];
            List[index - 1] = element;
            List[index] = itemToReplace;
            onValueChanged.OnNext(List);
        }
        public int Count => List.Count;
    }
}
