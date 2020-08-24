using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Converter
{
    public interface IJavascriptToCSharpConverter
    {
        IJsCsGlue GetCachedOrCreateBasic(IJavascriptObject key, Type targetType);
    }
}
