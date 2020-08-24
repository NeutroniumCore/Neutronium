using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding.CollectionChanges
{
    internal class CollectionChanges :  IComparer<IndividualCollectionChange>
    {
        internal IEnumerable<IndividualCollectionChange> IndividualChanges { get; }

        internal CollectionChanges(IJavascriptToGlueMapper jsCsBridgeCache, JavascriptCollectionChanges changes, Type targetedType)
        {
            IndividualChanges = changes.Changes
                           .Select(collectionChange => new IndividualCollectionChange(collectionChange, jsCsBridgeCache, targetedType))
                           .OrderBy(idc => idc, this).ToArray();
        }

        public int Compare(IndividualCollectionChange x, IndividualCollectionChange y)
        {
            if (x.CollectionChangeType != y.CollectionChangeType)
                return (x.CollectionChangeType == CollectionChangeType.Add) ? 1 : -1;

            return ((x.CollectionChangeType == CollectionChangeType.Add)? 1 : -1) * (x.Index - y.Index);
        }

        public IEnumerable<IJsCsGlue> GetGlues(CollectionChangeType @type) => 
            IndividualChanges.Where(ch => ch.CollectionChangeType == @type).Select(glue => glue.Object);
    }
}
