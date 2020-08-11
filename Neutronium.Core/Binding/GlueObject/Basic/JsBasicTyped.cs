using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal abstract class JsBasicTyped<T> : JsBasicTypedGcAgnostic<T>
    {
        public bool Release() => false;

        public IJsCsGlue AddRef() => (IJsCsGlue)this;

        protected JsBasicTyped(T value) : base(value)
        {
        }

        protected JsBasicTyped(IJavascriptObject jsValue, T value) : base(jsValue, value)
        {
        }
    }
}
