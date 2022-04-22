using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UniRxExtension
{
    public class ReactiveListNameSafe<T> : ReactiveList<T> where T : AiObjectModel
    {

        public override void Add(T element)
        {

            var numberOfIdenticalNames = Values
            .Where(e => e.Name.Contains(element.Name))
            .ToList()
            .Count;

            if (numberOfIdenticalNames > 0)
            {
                element.Name += "(" + numberOfIdenticalNames + ")";
            }

            base.Add(element);
        }

        public override void Add(IEnumerable<T> elements)
        {
            var aiObjectModels = elements.ToList();
            foreach (var element in aiObjectModels)
            {
                var numberOfIdenticalNames = Values
                    .Where(e => e.Name.Contains(element.Name))
                    .ToList()
                    .Count;

                if (numberOfIdenticalNames > 0)
                {
                    element.Name += "(" + numberOfIdenticalNames + ")";
                }
            }
            base.Add(aiObjectModels);
        }
    }
}