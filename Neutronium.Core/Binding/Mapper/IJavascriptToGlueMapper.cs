using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Mapper
{
    internal interface IJavascriptToGlueMapper
    {
        IGlueConvertible GetGlueConvertible(IJavascriptObject javascriptObject, Type targetType);
    }
}
