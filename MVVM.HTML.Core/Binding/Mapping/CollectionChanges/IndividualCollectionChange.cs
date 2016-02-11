using MVVM.HTML.Core.Binding.Mapping;
using System;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class IndividualCollectionChange
    {
        public IndividualCollectionChange(IndividualJavascriptCollectionChange change, IJavascriptToCSharpConverter jSCBridgeCache, Type targetType)
        {
            CollectionChangeType = change.CollectionChangeType;
             Index = change.Index;
             Object = jSCBridgeCache.GetCachedOrCreateBasic(change.Object, targetType);
        } 

        public CollectionChangeType  CollectionChangeType {get;private set;}

        public int Index { get; private set; }

        public IJSCSGlue Object { get; private set; }
    }
}
