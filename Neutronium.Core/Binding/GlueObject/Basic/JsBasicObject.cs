using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal sealed class JsBasicObject : IJsCsGlue
    {
        public IJavascriptObject JsValue { get; private set; }
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Basic;
        public uint JsId => 0;

        public IJsCsGlue AddRef() => this;

        public bool Release() => false;

        internal JsBasicObject(IJavascriptObject jsValue, object value)
        {
            CValue = value;
            JsValue = jsValue;
        }

        void IJsCsGlue.SetJsValue(IJavascriptObject value, IJavascriptSessionCache cache)
        {
            JsValue = value;
        }

        // Not need this class is only used when creating glueobject from
        // javascript session
        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) { }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit(this);
        }

        public void VisitChildren(Action<IJsCsGlue> visit) { }

        public override string ToString()
        {
            switch (CValue)
            {
                case null:
                    return "null";

                case string stringValue:
                    return JavascriptNamer.GetCreateExpressionDoubleQuote(stringValue);
            }

            object unBoxed = CValue;
            switch (unBoxed)
            {
                case DateTime dt:
                    return $@"""{dt.Year:0000}-{dt.Month:00}-{dt.Day:00}T{dt.Hour:00}:{dt.Minute:00}:{dt.Second:00}.{dt.Millisecond:000}Z""";

                case bool boolValue:
                    return $"{(boolValue ? "true" : "false")}";
            }

            return CValue.ToString();
        }

        public void BuilString(IDescriptionBuilder context)
        {
            context.Append(ToString());
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
