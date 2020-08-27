using System;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Mapper
{
    internal interface IJavascriptToGlueMapper
    {
        IJsCsGlue GetCachedOrCreateBasic(IJavascriptObject key, Type targetType);
    }
}
