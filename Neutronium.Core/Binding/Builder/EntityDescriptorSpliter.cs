using Neutronium.Core.Binding.GlueObject;
using System.Collections.Generic;
using System.Linq;

namespace Neutronium.Core.Binding.Builder
{
    public class EntityDescriptorSpliter<TIdentifier>
    {
        public int MaxCount { get; set; }

        internal IEnumerable<List<EntityDescriptor<TIdentifier>>> SplitParameters(IEnumerable<EntityDescriptor<TIdentifier>> _ParamBuilder) 
        {
            var index = 0;
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
                    return parametersCount < MaxCount;
                }).ToList();

                var localParametersCount = parameters.Count;
                if (localParametersCount == 0)
                    yield break;

                yield return parameters.GroupBy(item => item.father).Select(item => new EntityDescriptor<TIdentifier>(item.Key, item.Select(el => el.description)))
                                          .ToList();

                index += localParametersCount;
            }
            while (parametersCount >= MaxCount);
        }
    }
}
