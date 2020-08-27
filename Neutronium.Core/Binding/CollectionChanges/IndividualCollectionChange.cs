using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding.CollectionChanges
{
    internal class IndividualCollectionChange
    {
        internal IndividualCollectionChange(IndividualJavascriptCollectionChange change, IJavascriptToGlueMapper converter, Type targetType)
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
