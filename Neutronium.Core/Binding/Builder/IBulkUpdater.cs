using Neutronium.Core.Binding.GlueObject;
using System;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    internal interface IBulkUpdater
    {
        void BulkUpdateProperty(List<EntityDescriptor<string>> updates);

        void BulkUpdateArray(List<EntityDescriptor<int>> updates);
    }
}