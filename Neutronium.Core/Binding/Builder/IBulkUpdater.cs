using Neutronium.Core.Binding.GlueObject;
using System;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    internal interface IBulkUpdater
    {
        void BulkUpdateProperty(List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>> updates);

        void BulkUpdateArray(List<Tuple<IJSCSGlue, IList<IJSCSGlue>>> updates);
    }
}