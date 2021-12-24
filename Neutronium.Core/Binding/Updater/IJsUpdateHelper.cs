using System;
using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Updater
{
    public interface IJsUpdateHelper : IContextsManager
    {
        IWebSessionLogger Logger { get; }
        T GetCached<T>(object value) where T : class;
        T GetCachedFromJsObject<T>(IJavascriptObject value) where T : class;
        IJsCsGlue Map(object value);
        bool GetSimpleValue(IJavascriptObject value, out object res, Type targetType = null);
        IJsCsGlue MapJavascripObject(object csValue, IJavascriptObject jsValue);
        void UpdateOnUiContext(BridgeUpdater updater, ObjectChangesListener off);
        void UpdateOnJavascriptContext(BridgeUpdater updater, IJsCsGlue value);
        void UpdateOnJavascriptContext(BridgeUpdater updater, IList<IJsCsGlue> value);
        void UpdateOnJavascriptContext(BridgeUpdater updater);
    }
}