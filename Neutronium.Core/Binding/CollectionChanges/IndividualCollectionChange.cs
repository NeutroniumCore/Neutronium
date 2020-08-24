using System;
using Neutronium.Core.Binding.Converter;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding.CollectionChanges
{
    public class IndividualCollectionChange
    {
        public IndividualCollectionChange(IndividualJavascriptCollectionChange change, IJavascriptToGlueMapper converter, Type targetType)
        {
            CollectionChangeType = change.CollectionChangeType;
             Index = change.Index;
             Object = converter.GetCachedOrCreateBasic(change.Object, targetType);
        } 

        public CollectionChangeType  CollectionChangeType { get; }

        public int Index { get; }

        public IJsCsGlue Object { get; }
    }
}
