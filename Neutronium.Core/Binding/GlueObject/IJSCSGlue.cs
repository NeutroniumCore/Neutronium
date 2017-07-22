using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJSCSGlue
    {
        IJavascriptObject JSValue { get; }

        object CValue { get;}

        JsCsGlueType Type { get; }

        uint JsId { get;}

        IEnumerable<IJSCSGlue> GetChildren();

        void BuilString(DescriptionBuilder context);

        void GetBuildInstruction(IJavascriptObjectBuilder builder);

        void SetJSValue(IJavascriptObject value);

        void ApplyOnListenable(IObjectChangesListener listener);
    }
}
