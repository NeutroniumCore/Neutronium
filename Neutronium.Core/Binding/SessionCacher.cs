using System.Collections.Generic;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptFramework;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using MoreCollection.Extensions;
using System;

namespace Neutronium.Core.Binding
{
    internal class SessionCacher : IJavascriptSessionCache
    {
        private readonly IDictionary<object, IJSCSGlue> _FromCSharp = new Dictionary<object, IJSCSGlue>();
        private readonly IDictionary<uint, IJSCSGlue> _FromJavascript_Global = new Dictionary<uint, IJSCSGlue>();

        public void CacheFromCSharpValue(object key, IJSCSGlue value)
        {
            _FromCSharp.Add(key, value);
        }

        public void RemoveFromCSharpToJs(IJSCSGlue value)
        {
            var key = value.CValue;
            if (key == null)
                return;

            _FromCSharp.Remove(key);
        }

        public void RemoveFromJsToCSharp(IJSCSGlue value)
        {
            var id = value.JsId;
            if (id == 0)
                return;

            _FromJavascript_Global.Remove(id);
        }

        public void Cache(IJSCSGlue value)
        {
            var cashable = value as IJSCSCachableGlue;
            if (cashable != null)
                Cache(cashable);
            else
                _FromJavascript_Global.Add(value.JSValue.GetID(), value);
        }

        public void Cache(IJSCSCachableGlue cachableGlue)
        {
            var id = cachableGlue.CachableJSValue.GetID();
            if (id == 0)
                return;

            cachableGlue.SetJsId(id);
            _FromJavascript_Global[id] = cachableGlue;
        }

        private void CacheGlobal(IJavascriptObject jsobject, IJSCSMappedBridge ibo)
        {
            var id = jsobject.GetID();
            if (id == 0)
                return;

            ibo.SetJsId(id);
            _FromJavascript_Global[id] = ibo;
        }

        public IJSCSGlue GetCached(object key)
        {
            return _FromCSharp.GetOrDefault(key);
        }

        public IJSCSGlue GetCached(IJavascriptObject globalkey) 
        {
            var id = globalkey.GetID();
            return (id == 0) ? null : _FromJavascript_Global.GetOrDefault(id);
        }

        public IJSCSGlue GetCached(uint id)
        {
            return _FromJavascript_Global.GetOrDefault(id);
        }

        public IJavascriptObjectInternalMapper GetMapper(IJSCSMappedBridge root)
        {
            return new JavascriptMapper(root, Update, RegisterMapping, RegisterCollectionMapping);
        }

        internal void Update(IJSCSMappedBridge observableBridge, IJavascriptObject jsobject)
        {
            observableBridge.SetMappedJSValue(jsobject);
            CacheGlobal(jsobject, observableBridge);
        }

        internal void RegisterMapping(IJavascriptObject father, string att, IJavascriptObject child)
        {
            var global = GetCached(father);
            if (global is JSCommand)
                return;

            var jso = (JsGenericObject)global;
            Update(jso.Attributes[att] as IJSCSMappedBridge, child);
        }

        internal void RegisterCollectionMapping(IJavascriptObject jsFather, string att, int index, IJavascriptObject child)
        {
            var father = GetCached(jsFather);
            var jsos = (att == null) ? father : ((JsGenericObject)father).Attributes[att];

            Update(((JSArray)jsos).Items[index] as IJSCSMappedBridge, child);
        }
    }
}
