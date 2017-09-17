using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    internal abstract class JsNumber<T> where T : struct
    {
        public IJavascriptObject JsValue { get; private set; }
        public T NumberValue { get; }
        public object CValue => NumberValue;
        public JsCsGlueType Type => JsCsGlueType.Basic;
        public uint JsId => 0;

        public bool Release() => false;

        public IJsCsGlue AddRef() => (IJsCsGlue)this;

        internal JsNumber(T value)
        {
            NumberValue = value;
        }

        protected void SetJsValue(IJavascriptObject value)
        {
            JsValue = value;
        }

        internal JsNumber(IJavascriptObject jsValue, T value)
        {
            NumberValue = value;
            JsValue = jsValue;
        }

        public void VisitDescendants(Func<IJsCsGlue, bool> visit)
        {
            visit((IJsCsGlue)this);
        }

        public void VisitChildren(Action<IJsCsGlue> visit) { }

        public override string ToString()
        {
            return NumberValue.ToString();
        }

        public void BuilString(DescriptionBuilder context)
        {
            context.Append(ToString());
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
