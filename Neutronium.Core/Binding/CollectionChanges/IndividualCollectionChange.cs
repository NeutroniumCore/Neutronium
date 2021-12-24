using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Binding.Updater;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding.CollectionChanges
{
    public class IndividualCollectionChange
    {
        internal CollectionChangeType CollectionChangeType { get; }
        internal int Index { get; }
        internal IGlueMapable Mapable { get; }
        internal IJsCsGlue Glue { get; private set; }

        internal object Object => Mapable.Source;

        internal IndividualCollectionChange(IndividualJavascriptCollectionChange change, IJavascriptToGlueMapper mapper, Type targetType)
        {
            CollectionChangeType = change.CollectionChangeType;
             Index = change.Index;
             Mapable = mapper.GetGlueConvertible(change.Object, targetType);
        }

        internal void ComputeGlue(IJsUpdateHelper mapper)
        {
            Glue = Mapable.Map(mapper);
        }
    }
}
