using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public class EntityArraySpliter
    {
        public int MaxCount { get; set; }

        internal IEnumerable<List<ArrayDescriptor>> SplitParameters(IEnumerable<ArrayDescriptor> data)
        {
            int parametersCount = 0;

            var list = new List<ArrayDescriptor>();
            var maxCountInContext = MaxCount - 2;

            foreach (var element in data)
            {
                var childrenCount = element.OrdenedChildren.Count;
                var tentative = parametersCount + 1 + childrenCount;
                var delta = tentative - MaxCount;

                if ((delta == -1) || (delta == -2))
                {
                    list.Add(element);
                    yield return list;

                    list = new List<ArrayDescriptor>();
                    parametersCount = 0;
                    continue;
                }

                if (delta >= 0)
                {
                    var maxToTake = maxCountInContext - parametersCount;
                    list.Add(new ArrayDescriptor(element.Father, element.OrdenedChildren.Take(maxToTake).ToArray()));
                    yield return list;

                    var count = childrenCount - maxToTake;
                    var i = 0;
                    for (i = 0; i < count / maxCountInContext; i++)
                    {
                        list = new List<ArrayDescriptor>();
                        var skip = maxToTake + maxCountInContext * i;
                        list.Add(new ArrayDescriptor(element.Father, element.OrdenedChildren.Skip(skip).Take(maxCountInContext).ToArray(), skip));
                        yield return list;
                    }

                    var skipped = (maxToTake + maxCountInContext * i);
                    list = new List<ArrayDescriptor>();
                    parametersCount = childrenCount - skipped;
                    if (skipped < childrenCount)
                    {
                        list.Add(new ArrayDescriptor(element.Father, element.OrdenedChildren.Skip(skipped).ToArray(), skipped));
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
