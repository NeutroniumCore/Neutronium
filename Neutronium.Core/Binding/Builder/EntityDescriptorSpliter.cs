using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    public class EntityDescriptorSpliter
    {
        public int MaxCount { get; set; }

        internal IEnumerable<List<ObjectDescriptor>> SplitParameters(IEnumerable<ObjectDescriptor> data)
        {
            var parametersCount = 0;

            var list = new List<ObjectDescriptor>();
            var maxCountInContext = MaxCount - 2;

            foreach (var element in data)
            {
                var childrenCount = element.AttributeValues.Length;
                var tentative = parametersCount + 1 + childrenCount;
                var delta = tentative - MaxCount;

                if ((delta == -1) || (delta == -2))
                {
                    list.Add(element);
                    yield return list;

                    list = new List<ObjectDescriptor>();
                    parametersCount = 0;
                    continue;
                }

                if (delta >= 0)
                {
                    var maxToTake = maxCountInContext - parametersCount;
                    list.Add(element.Take(maxToTake));
                    yield return list;

                    var count = childrenCount - maxToTake;
                    var i = 0;
                    for (i = 0; i < count / maxCountInContext; i++)
                    {
                        list = new List<ObjectDescriptor>
                        {
                            element.Split(maxToTake + maxCountInContext * i, maxCountInContext)
                        };
                        yield return list;
                    }

                    var skipped = (maxToTake + maxCountInContext * i);
                    list = new List<ObjectDescriptor>();
                    parametersCount = childrenCount - skipped;
                    if (skipped < childrenCount)
                    {
                        list.Add(element.Skip(skipped));
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
