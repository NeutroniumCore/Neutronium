using System;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal sealed class JsNull : IBasicJsCsGlue
    {
        public IJavascriptObject JsValue { get; private set; }
        public object CValue => null;
        public JsCsGlueType Type => JsCsGlueType.Basic;
        public uint JsId => 0;

        public bool Release() => false;

        public IJsCsGlue AddRef() => this;

        void IJsCsGlue.SetJsValue(IJavascriptObject value, IJavascriptSessionCache cache)
        {
            JsValue = value;
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit(this);
        }

        public void VisitChildren(Action<IJsCsGlue> visit) { }

        public void BuilString(DescriptionBuilder context)
        {
            context.Append(ToString());
        }

        public override string ToString() => GetCreationCode();

        public void ApplyOnListenable(IObjectChangesListener listener) { }

        public string GetCreationCode() => "null";

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestNullCreation();
    }
}
