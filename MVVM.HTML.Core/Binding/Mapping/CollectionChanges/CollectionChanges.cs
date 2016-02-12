using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVM.HTML.Core.Binding.Mapping;


namespace MVVM.HTML.Core.HTMLBinding
{
    public class CollectionChanges :  IComparer<IndividualCollectionChange>
    {
        private readonly IJavascriptToCSharpConverter _IJSCBridgeCache;
        private readonly Type _TargetedType;
        private readonly JavascriptCollectionChanges _Changes;

        public CollectionChanges(IJavascriptToCSharpConverter iJSCBridgeCache, JavascriptCollectionChanges changes, Type iTargetedType)
        {
            _IJSCBridgeCache = iJSCBridgeCache;
            _TargetedType = iTargetedType;
            _Changes = changes;
        }


        public IEnumerable<IndividualCollectionChange> IndividualChanges 
        {
            get
            {
                return _Changes.Changes
                            .Select(jvchnage => new IndividualCollectionChange(jvchnage, _IJSCBridgeCache, _TargetedType))
                            .OrderBy(idc => idc, this);
            }
        }

        public int Compare(IndividualCollectionChange x, IndividualCollectionChange y)
        {
            if (x.CollectionChangeType != y.CollectionChangeType)
                return (x.CollectionChangeType == CollectionChangeType.Add) ? 1 : -1;

            return ((x.CollectionChangeType == CollectionChangeType.Add)? 1 : -1) * (x.Index - y.Index);
        }
    }
}
