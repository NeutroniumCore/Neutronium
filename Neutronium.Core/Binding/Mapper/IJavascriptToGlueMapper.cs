using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Mapper
{
    internal interface IJavascriptToGlueMapper
    {
        IGlueMapable GetGlueConvertible(IJavascriptObject javascriptObject, Type targetType);
    }
}
