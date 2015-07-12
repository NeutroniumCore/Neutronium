using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.HTMLBinding
{
    public interface IJSCBridgeCache
    {
        void Cache(object key, IJSCSGlue value);

        void CacheLocal(object key, IJSCSGlue value);

        void RegisterInSession(object key, Action<IJSCSGlue> Continue);

        IJSCSGlue GetCached(object key);

        IJSCSGlue GetCached(CefV8Value key);

        IJSCSGlue GetCachedOrCreateBasic(CefV8Value key, Type iTargetType);

    }
}
