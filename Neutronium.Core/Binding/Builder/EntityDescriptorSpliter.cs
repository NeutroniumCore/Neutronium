using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    public class Parameters
    {
        public List<ObjectDescriptor> ObjectDescriptors { get; } = new List<ObjectDescriptor>();

        public int Count { get; set; }
    }


    public class EntityDescriptorSpliter
    {
        public int MaxCount { get; set; }

        public IEnumerable<Parameters> SplitParameters(IEnumerable<ObjectDescriptor> data)
        {
            var parametersCount = 0;

            var list = new Parameters();
            var maxCountInContext = MaxCount - 2;

            foreach (var element in data)
            {
                var childrenCount = element.AttributeValues.Count;
                var tentative = parametersCount + 1 + childrenCount;
                var delta = tentative - MaxCount;

                if ((delta == -1) || (delta == -2))
                {
                    list.ObjectDescriptors.Add(element);
                    list.Count = tentative;
                    yield return list;

                    list = new Parameters();
                    parametersCount = 0;
                    continue;
                }

                if (delta >= 0)
                {
                    var maxToTake = maxCountInContext - parametersCount;
                    list.Count = maxCountInContext + 1;
                    list.ObjectDescriptors.Add(element.Take(maxToTake));
                    yield return list;

                    var count = childrenCount - maxToTake;
                    var i = 0;
                    for (i = 0; i < count / maxCountInContext; i++)
                    {
                        list = new Parameters();
                        list.ObjectDescriptors.Add(element.Split(maxToTake + maxCountInContext * i, maxCountInContext));
                        list.Count = maxCountInContext + 1;
                        yield return list;
                    }

                    var skipped = (maxToTake + maxCountInContext * i);
                    list = new Parameters();
                    parametersCount = childrenCount - skipped;
                    if (skipped < childrenCount)
                    {
                        list.ObjectDescriptors.Add(element.Skip(skipped));
                        parametersCount++;
                    }
                    continue;
                }

                parametersCount = tentative;
                list.ObjectDescriptors.Add(element);
            }

            if (list.ObjectDescriptors.Count != 0)
            {
                list.Count = parametersCount;
                yield return list;
            }              
        }
    }
}
