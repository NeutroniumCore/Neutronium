using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding
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
