using System;
using Neutronium.Core.Binding.Listeners;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal abstract class JsBasicTypedGcAgnostic<T>
    {
        public IJavascriptObject JsValue { get; private set; }
        public T TypedValue { get; }
        public object CValue => TypedValue;
        public JsCsGlueType Type => JsCsGlueType.Basic;
        public uint JsId => 0;

        protected JsBasicTypedGcAgnostic(T value)
        {
            TypedValue = value;
        }

        protected JsBasicTypedGcAgnostic(IJavascriptObject jsValue, T value)
        {
            TypedValue = value;
            JsValue = jsValue;
        }

        protected void SetJsValue(IJavascriptObject value)
        {
            JsValue = value;
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit((IJsCsGlue)this);
        }

        public void VisitChildren(Action<IJsCsGlue> visit) { }

        public void BuilString(IDescriptionBuilder context)
        {
            context.Append(ToString());
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
