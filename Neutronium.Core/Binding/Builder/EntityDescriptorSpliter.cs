using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public class EntityDescriptorSpliter<TIdentifier>
    {
        public int MaxCount { get; set; }

        internal IEnumerable<List<EntityDescriptor<TIdentifier>>> SplitParameters(IEnumerable<EntityDescriptor<TIdentifier>> data)
        {
            int parametersCount = 0;

            var list = new List<EntityDescriptor<TIdentifier>>();
            var maxCountInContext = MaxCount - 2;

            foreach (var element in data)
            {
                var childrenCount = element.ChildrenDescription.Length;
                var tentative = parametersCount + 1 + childrenCount;
                var delta = tentative - MaxCount;

                if ((delta == -1) || (delta == -2))
                {
                    list.Add(element);
                    yield return list;

                    list = new List<EntityDescriptor<TIdentifier>>();
                    parametersCount = 0;
                    continue;
                }

                if (delta >= 0)
                {
                    var maxToTake = maxCountInContext - parametersCount;
                    list.Add(new EntityDescriptor<TIdentifier>(element.Father, element.ChildrenDescription.Take(maxToTake)));
                    yield return list;

                    var count = childrenCount - maxToTake;
                    var i = 0;
                    for (i = 0; i < count / maxCountInContext; i++)
                    {
                        list = new List<EntityDescriptor<TIdentifier>>();
                        list.Add(new EntityDescriptor<TIdentifier>(element.Father, element.ChildrenDescription.Skip(maxToTake + maxCountInContext * i).Take(maxCountInContext)));
                        yield return list;
                    }

                    var skipped = (maxToTake + maxCountInContext * i);
                    list = new List<EntityDescriptor<TIdentifier>>();
                    parametersCount = childrenCount - skipped;
                    if (skipped < childrenCount)
                    {
                        list.Add(new EntityDescriptor<TIdentifier>(element.Father, element.ChildrenDescription.Skip(skipped)));
                        parametersCount++;
                    }
                    continue;
                }

                parametersCount = tentative;
                list.Add(element);
            }

            if (list.Count != 0)
                yield return list;
        }
    }
}
