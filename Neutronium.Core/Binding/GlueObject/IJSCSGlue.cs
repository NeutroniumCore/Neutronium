using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJSCSGlue
    {
        IJavascriptObject JSValue { get; }

        object CValue { get;}

        JsCsGlueType Type { get; }

        IEnumerable<IJSCSGlue> GetChildren();

        void BuilString(DescriptionBuilder context);

        JSBuilder GetJSBuilder();
    }
}
