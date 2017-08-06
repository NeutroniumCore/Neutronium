using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System.Collections.Generic;

namespace Neutronium.Core.Binding.Builder
{
    internal interface IBulkUpdater
    {
        IJavascriptObject CommandConstructor { get; }
        IJavascriptObject ExecutableConstructor { get; }

        void BulkUpdateProperty(IEnumerable<EntityDescriptor<string>> updates);

        void BulkUpdateArray(IEnumerable<EntityDescriptor<int>> updates);
    }
}