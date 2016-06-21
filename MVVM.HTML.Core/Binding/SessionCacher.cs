using System.Collections.Generic;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;
using System;

namespace MVVM.HTML.Core.Binding
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
            Console.WriteLine(globalkey.GetID());
            if (!globalkey.HasRelevantId())
                return null;

            return _FromJavascript_Global.GetOrDefault(globalkey.GetID());
        }

        public IJSCSGlue GetCachedLocal(IJavascriptObject localkey)
        {
            if (!localkey.HasRelevantId())
                return null;

            return _FromJavascript_Local.GetOrDefault(localkey.GetID());
        }

        public IJavascriptObjectMapper GetMapper(IJSObservableBridge root)
        {
            return new JavascriptMapper(root, this);
        }

        private class JavascriptMapper : IJavascriptObjectMapper
        {
            private readonly IJSObservableBridge _Root;
            private readonly SessionCacher _LiveMapper;
            private readonly TaskCompletionSource<object> _TCS = new TaskCompletionSource<object>();
            public JavascriptMapper(IJSObservableBridge root, SessionCacher father)
            {
                _LiveMapper = father;
                _Root = root;
            }

            public void MapFirst(IJavascriptObject iRoot)
            {
                _LiveMapper.Update(_Root, iRoot);
            }

            public void Map(IJavascriptObject iFather, string att, IJavascriptObject iChild)
            {
                _LiveMapper.RegisterMapping(iFather, att, iChild);
            }

            public void MapCollection(IJavascriptObject iFather, string att, int index, IJavascriptObject iChild)
            {
                _LiveMapper.RegisterCollectionMapping(iFather, att, index, iChild);
            }

            public Task UpdateTask { get { return _TCS.Task; } }

            public void EndMapping(IJavascriptObject iRoot)
            {
                _TCS.TrySetResult(null);
            }
        }

        private void Update(IJSObservableBridge ibo, IJavascriptObject jsobject)
        {
            ibo.SetMappedJSValue(jsobject);
            CacheGlobal(jsobject, ibo);
        }

        private void RegisterMapping(IJavascriptObject iFather, string att, IJavascriptObject iChild)
        {
            var global = GetGlobalCached(iFather);
            if (global is JSCommand)
                return;

            var jso = (JSGenericObject)global;
            Update(jso.Attributes[att] as IJSObservableBridge, iChild);
        }

        private void RegisterCollectionMapping(IJavascriptObject iFather, string att, int index, IJavascriptObject iChild)
        {
            var father = GetGlobalCached(iFather);
            var jsos = (att == null) ? father : ((JSGenericObject)father).Attributes[att];

            Update(((JSArray)jsos).Items[index] as IJSObservableBridge, iChild);
        }
    }
}
