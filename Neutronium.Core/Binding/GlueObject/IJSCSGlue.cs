using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    public interface IJsCsGlue
    {
        IJavascriptObject JsValue { get; }

        object CValue { get; }

        JsCsGlueType Type { get; }

        uint JsId { get; }

        IJsCsGlue AddRef();

        bool Release();

        void BuilString(DescriptionBuilder context);

        void RequestBuildInstruction(IJavascriptObjectBuilder builder);

        void SetJsValue(IJavascriptObject value);

        void ApplyOnListenable(IObjectChangesListener listener);

        void VisitDescendants(Func<IJsCsGlue, bool> visit);

        void VisitChildren(Action<IJsCsGlue> visit);
    }
}
