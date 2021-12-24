using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Mapper
{
    public interface IJavascriptToGlueMapper
    {
        IGlueMapable GetGlueConvertible(IJavascriptObject javascriptObject, Type targetType);
    }
}
