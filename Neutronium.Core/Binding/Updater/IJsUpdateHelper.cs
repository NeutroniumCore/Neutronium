using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Updater
{
    internal interface IJsUpdateHelper : IContextsManager
    {
        T GetCached<T>(object value) where T : class;
        T GetCachedFromJsObject<T>(IJavascriptObject value) where T : class;
        IJsCsGlue Map(object value);
        void UpdateOnUiContext(BridgeUpdater updater, ObjectChangesListener off);
        void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue value);
        void UpdateOnJavascriptContext(BridgeUpdater updater, IList<IJsCsGlue> value);
        void UpdateOnJavascriptContext(BridgeUpdater updater);
    }
}