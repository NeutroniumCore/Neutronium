using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJsCsGlue
    {
        IJavascriptObject JsValue { get; }

        object CValue { get;}

        JsCsGlueType Type { get; }

        uint JsId { get;}

        IEnumerable<IJsCsGlue> Children { get; }

        void BuilString(DescriptionBuilder context);

        void RequestBuildInstruction(IJavascriptObjectBuilder builder);

        void SetJsValue(IJavascriptObject value);

        void ApplyOnListenable(IObjectChangesListener listener);
    }
}
