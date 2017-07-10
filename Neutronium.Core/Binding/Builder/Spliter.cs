using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public class Spliter<TIdentifier>
    {
        public int AddicionalParameterCount { get; set; } = 1;
        public int MaxCount { get; set; } = 200000;

        internal IEnumerable<List<ChildrenDescriptor<TIdentifier>>> SplitParameters(IEnumerable<ChildrenDescriptor<TIdentifier>> _ParamBuilder) 
        {
            var index = 0;
            var maxCount = MaxCount - AddicionalParameterCount;
            int parametersCount;

            var data = _ParamBuilder.SelectMany(item => item.ChildrenDescription, (item, description) => new { father = item.Father, description }).ToList();
            do
            {
                parametersCount = 0;
                var fathers = new HashSet<IJSCSGlue>();
                var parameters = data.Skip(index).TakeWhile((entity) => {
                    parametersCount += 1;
                    if (fathers.Add(entity.father))
                        parametersCount++;
                    return parametersCount < maxCount;
                }).ToList();

                var localParametersCount = parameters.Count;
                if (localParametersCount == 0)
                    yield break;

                yield return parameters.GroupBy(item => item.father).Select(item => new ChildrenDescriptor<TIdentifier>(item.Key, item.Select(el => el.description)))
                                          .ToList();

                index += localParametersCount;
            }
            while (parametersCount >= maxCount);
        }
    }
}
