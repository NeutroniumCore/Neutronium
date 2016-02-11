using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core.HTMLBinding
{
    public interface IJavascriptSessionCache
    {
        void Cache(object key, IJSCSGlue value);

        void CacheLocal(object key, IJSCSGlue value);

        IJSCSGlue GetCached(object key);

        IJSCSGlue GetGlobalCached(IJavascriptObject key);

        IJSCSGlue GetCachedLocal(IJavascriptObject localkey);
    }
}
