using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal abstract class JsBasicTyped<T> : JsBasicTypedGCAgnostic<T>
    {
        public bool Release() => false;

        public IJsCsGlue AddRef() => (IJsCsGlue)this;

        internal JsBasicTyped(T value) : base(value)
        {
        }

        internal JsBasicTyped(IJavascriptObject jsValue, T value) : base(jsValue, value)
        {
        }
    }
}
