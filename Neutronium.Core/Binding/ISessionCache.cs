using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding
{
    public interface ISessionCache : ICSharpToJsCache
    {
        void Cache(IJsCsGlue value);

        void Cache(IJsCsCachableGlue cachableGlue);

        void RemoveFromCSharpToJs(IJsCsGlue value);

        void RemoveFromJsToCSharp(IJsCsGlue value);

        IJsCsGlue GetCached(IJavascriptObject globalKey);

        IJsCsGlue GetCached(uint id);
    }
}
