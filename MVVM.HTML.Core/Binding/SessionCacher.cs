using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.Infra;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptFramework;

namespace Neutronium.Core.Binding
{
    internal class SessionCacher : IJavascriptSessionCache
    {
        private readonly IDictionary<object, IJSCSGlue> _FromCSharp = new Dictionary<object, IJSCSGlue>();
        private readonly IDictionary<uint, IJSCSGlue> _FromJavascript_Global = new Dictionary<uint, IJSCSGlue>();
        private readonly IDictionary<uint, IJSCSGlue> _FromJavascript_Local = new Dictionary<uint, IJSCSGlue>();

        public void Cache(object key, IJSCSGlue value)
        {
            _FromCSharp.Add(key, value);
        }

        public void CacheLocal(object key, IJSCSGlue value)
        {
            _FromCSharp.Add(key, value);
            _FromJavascript_Local.Add(value.JSValue.GetID(), value);
        }

        private void CacheGlobal(IJavascriptObject jsobject, IJSObservableBridge ibo)
        {
            if (jsobject.HasRelevantId())
                _FromJavascript_Global[jsobject.GetID()] = ibo;
        }

        public IJSCSGlue GetCached(object key)
        {
            return _FromCSharp.GetOrDefault(key);
        }

        public IJSCSGlue GetGlobalCached(IJavascriptObject globalkey) 
        {
            return !globalkey.HasRelevantId() ? null : _FromJavascript_Global.GetOrDefault(globalkey.GetID());
        }

        public IJSCSGlue GetCachedLocal(IJavascriptObject localkey) 
        {
            return !localkey.HasRelevantId() ? null : _FromJavascript_Local.GetOrDefault(localkey.GetID());
        }

        public IJavascriptObjectInternalMapper GetMapper(IJSObservableBridge root)
        {
            return new JavascriptMapper(root, CacheGlobal, Update, RegisterMapping, RegisterCollectionMapping);
        }

        internal void Update(IJSObservableBridge observableBridge, IJavascriptObject jsobject)
        {
            observableBridge.SetMappedJSValue(jsobject);
            CacheGlobal(jsobject, observableBridge);
        }

        internal void RegisterMapping(IJavascriptObject father, string att, IJavascriptObject child)
        {
            var global = GetGlobalCached(father);
            if (global is JSCommand)
                return;

            var jso = (JsGenericObject)global;
            Update(jso.Attributes[att] as IJSObservableBridge, child);
        }

        internal void RegisterCollectionMapping(IJavascriptObject jsFather, string att, int index, IJavascriptObject child)
        {
            var father = GetGlobalCached(jsFather);
            var jsos = (att == null) ? father : ((JsGenericObject)father).Attributes[att];

            Update(((JSArray)jsos).Items[index] as IJSObservableBridge, child);
        }
    }
}
