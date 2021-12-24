using System;
using System.Collections.Generic;
using System.Linq;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Mapper;
using Neutronium.Core.Binding.Updater;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding.CollectionChanges
{
    public class CollectionChanges :  IComparer<IndividualCollectionChange>
    {
        internal IEnumerable<IndividualCollectionChange> IndividualChanges { get; }

        internal CollectionChanges(IJavascriptToGlueMapper jsCsBridgeCache, JavascriptCollectionChanges changes, Type targetedType)
        {
            IndividualChanges = changes.Changes
                           .Select(collectionChange => new IndividualCollectionChange(collectionChange, jsCsBridgeCache, targetedType))
                           .OrderBy(idc => idc, this).ToArray();
        }

        internal void ComputeGlues(IJsUpdateHelper mapper)
        {
            IndividualChanges.ForEach(change => change.ComputeGlue(mapper));
        }

        public int Compare(IndividualCollectionChange x, IndividualCollectionChange y)
        {
            if (x.CollectionChangeType != y.CollectionChangeType)
                return (x.CollectionChangeType == CollectionChangeType.Add) ? 1 : -1;

            return ((x.CollectionChangeType == CollectionChangeType.Add)? 1 : -1) * (x.Index - y.Index);
        }

        public IEnumerable<IJsCsGlue> GetGlues(CollectionChangeType @type) => IndividualChanges.Where(ch => ch.CollectionChangeType == @type).Select(change => change.Glue);
    }
}
