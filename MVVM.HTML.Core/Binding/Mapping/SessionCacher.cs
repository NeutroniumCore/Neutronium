using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.Binding.Mapping
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

        public void CacheGlobal(IJavascriptObject jsobject, IJSObservableBridge ibo)
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
    }
}
