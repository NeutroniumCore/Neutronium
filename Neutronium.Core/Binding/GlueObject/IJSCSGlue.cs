using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJSCSGlue
    {
        IJavascriptObject JSValue { get; }

        object CValue { get;}

        JsCsGlueType Type { get; }

        IEnumerable<IJSCSGlue> GetChildren();

        void BuilString(DescriptionBuilder context);

        void ComputeJavascriptValue(IWebView factory, IJavascriptSessionCache cache);
    }
}
