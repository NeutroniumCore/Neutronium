using Neutronium.Core.Binding.GlueObject;
using System;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    internal interface IBulkPropertyUpdater
    {
        void BulkUpdateProperty(List<Tuple<IJSCSGlue, IReadOnlyDictionary<string, IJSCSGlue>>> updates);
    }
}