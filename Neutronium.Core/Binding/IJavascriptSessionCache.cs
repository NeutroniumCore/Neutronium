using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding
{
    public interface IJavascriptSessionCache
    {
        void CacheFromCSharpValue(object key, IJSCSGlue value);

        void Cache(IJSCSGlue value);

        void Cache(IJSCSCachableGlue cachableGlue);

        void RemoveFromCSharpToJs(IJSCSGlue value);

        void RemoveFromJsToCSharp(IJSCSGlue value);

        IJSCSGlue GetCached(object key);

        IJSCSGlue GetCached(IJavascriptObject globalkey);

        IJSCSGlue GetCached(uint id);
    }
}
