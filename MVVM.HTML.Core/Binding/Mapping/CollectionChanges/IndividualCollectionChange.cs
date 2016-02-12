using MVVM.HTML.Core.Binding.Mapping;
using System;

namespace MVVM.HTML.Core.HTMLBinding
{
    public class IndividualCollectionChange
    {
        public IndividualCollectionChange(IndividualJavascriptCollectionChange change, IJavascriptToCSharpConverter converter, Type targetType)
        {
            CollectionChangeType = change.CollectionChangeType;
             Index = change.Index;
             Object = converter.GetCachedOrCreateBasic(change.Object, targetType);
        } 

        public CollectionChangeType  CollectionChangeType {get;private set;}

        public int Index { get; private set; }

        public IJSCSGlue Object { get; private set; }
    }
}
