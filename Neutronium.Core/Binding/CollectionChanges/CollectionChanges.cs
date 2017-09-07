using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding.CollectionChanges
{
    public class CollectionChanges :  IComparer<IndividualCollectionChange>
    {
        public IEnumerable<IndividualCollectionChange> IndividualChanges { get; }

        internal CollectionChanges(IJavascriptToCSharpConverter jsCsBridgeCache, JavascriptCollectionChanges changes, Type targetedType)
        {
            IndividualChanges = changes.Changes
                           .Select(jvchnage => new IndividualCollectionChange(jvchnage, jsCsBridgeCache, targetedType))
                           .OrderBy(idc => idc, this).ToArray();
        }

        public int Compare(IndividualCollectionChange x, IndividualCollectionChange y)
        {
            if (x.CollectionChangeType != y.CollectionChangeType)
                return (x.CollectionChangeType == CollectionChangeType.Add) ? 1 : -1;

            return ((x.CollectionChangeType == CollectionChangeType.Add)? 1 : -1) * (x.Index - y.Index);
        }
    }
}
