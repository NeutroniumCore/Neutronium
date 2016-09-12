using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.Binding
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
