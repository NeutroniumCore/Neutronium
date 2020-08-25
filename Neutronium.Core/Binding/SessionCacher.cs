using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;
using Neutronium.Core.Binding.GlueObject.Executable;
using Neutronium.Core.Binding.JavascriptFrameworkMapper;

namespace Neutronium.Core.Binding
{
    internal class SessionCacher : IJavascriptSessionCache
    {
        private readonly IDictionary<object, IJsCsGlue> _FromCSharp = new Dictionary<object, IJsCsGlue>();
        private readonly IDictionary<uint, IJsCsGlue> _FromJavascriptGlobal = new Dictionary<uint, IJsCsGlue>();

        public IEnumerable<IJsCsGlue> AllElementsUiContext => _FromCSharp.Values;

        public void CacheFromCSharpValue(object key, IJsCsGlue value)
        {
            _FromCSharp.Add(key, value);
        }

        public void RemoveFromCSharpToJs(IJsCsGlue value)
        {
            var key = value.CValue;
            if (key == null)
                return;

            _FromCSharp.Remove(key);
        }

        public void RemoveFromJsToCSharp(IJsCsGlue value)
        {
            var id = value.JsId;
            if (id == 0)
                return;

            _FromJavascriptGlobal.Remove(id);
        }

        public void Cache(IJsCsGlue value)
        {
            _FromJavascriptGlobal.Add(value.JsValue.GetID(), value);
        }

        public void Cache(IJsCsCachableGlue cachableGlue)
        {
            var id = cachableGlue.CachableJsValue.GetID();
            if (id == 0)
                return;

            cachableGlue.SetJsId(id);
            _FromJavascriptGlobal[id] = cachableGlue;
        }

        private void CacheGlobal(IJavascriptObject jsObject, IJsCsMappedBridge ibo)
        {
            var id = jsObject.GetID();
            if (id == 0)
                return;

            ibo.SetJsId(id);
            _FromJavascriptGlobal[id] = ibo;
        }

        public IJsCsGlue GetCached(object key)
        {
            return _FromCSharp.GetOrDefault(key);
        }

        public IJsCsGlue GetCached(IJavascriptObject globalKey) 
        {
            var id = globalKey.GetID();
            return (id == 0) ? null : _FromJavascriptGlobal.GetOrDefault(id);
        }

        public IJsCsGlue GetCached(uint id)
        {
            return _FromJavascriptGlobal.GetOrDefault(id);
        }

        public IJavascriptObjectInternalMapper GetMapper(IJsCsMappedBridge root)
        {
            return new JavascriptMapper(root, Update, RegisterMapping, RegisterCollectionMapping);
        }

        internal void Update(IJsCsMappedBridge observableBridge, IJavascriptObject jsObject)
        {
            observableBridge.SetMappedJsValue(jsObject);
            CacheGlobal(jsObject, observableBridge);
        }

        internal void RegisterMapping(IJavascriptObject father, string att, IJavascriptObject child)
        {
            var global = GetCached(father);
            if (global is JsCommand)
                return;

            var jso = (JsGenericObject)global;
            Update(jso.GetAttribute(att) as IJsCsMappedBridge, child);
        }

        internal void RegisterCollectionMapping(IJavascriptObject jsFather, string att, int index, IJavascriptObject child)
        {
            var father = GetCached(jsFather);
            var jsObject = (att == null) ? father : ((JsGenericObject)father).GetAttribute(att);

            Update(((JsArray)jsObject).Items[index] as IJsCsMappedBridge, child);
        }
    }
}
