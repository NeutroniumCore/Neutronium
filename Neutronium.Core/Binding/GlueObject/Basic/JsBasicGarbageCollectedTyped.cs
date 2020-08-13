using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal abstract class JsBasicGarbageCollectedTyped<T> : JsBasicTypedGcAgnostic<T>
    {
        private int _Count = 0;

        public bool Release() => (--_Count == 0);

        public IJsCsGlue AddRef()
        {
            _Count++;
            return (IJsCsGlue) this;
        }

        protected JsBasicGarbageCollectedTyped(T value) : base(value)
        {
        }

        protected JsBasicGarbageCollectedTyped(IJavascriptObject jsValue, T value) : base(jsValue, value)
        {
        }
    }
}
