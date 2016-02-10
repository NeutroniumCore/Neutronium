using MVVM.HTML.Core.V8JavascriptObject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJSCBridgeCache
    {
        void Cache(object key, IJSCSGlue value);

        void CacheLocal(object key, IJSCSGlue value);

        IJSCSGlue GetCached(object key);

        IJSCSGlue GetGlobalCached(IJavascriptObject key);

        IJSCSGlue GetCachedLocal(IJavascriptObject localkey);
    }
}
