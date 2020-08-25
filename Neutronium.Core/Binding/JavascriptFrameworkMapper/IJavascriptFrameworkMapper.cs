using System.Collections.Generic;
using System.Threading.Tasks;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.JavascriptFrameworkMapper
{
    internal interface IJavascriptFrameworkMapper
    {
        Task<IJavascriptObject> UpdateJavascriptObject(IJsCsGlue jsValueRoot);

        void UpdateOnJavascriptContext(BridgeUpdater updater, IList<IJsCsGlue> values);

        void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue values);

        void UpdateOnJavascriptContext(BridgeUpdater updater);
    }
}
