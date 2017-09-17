using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    internal abstract class JsNumber<T>: JsBasicTyped<T> where T : struct
    {
        internal JsNumber(T value):base(value) { }

        internal JsNumber(IJavascriptObject jsValue, T value): base(jsValue, value) { }

        public override string ToString() => TypedValue.ToString();
    }

    internal sealed class JsInt : JsNumber<int>, IBasicJsCsGlue
    {
        public JsInt(int value) : base(value) { }

        public JsInt(IJavascriptObject jsValue, int value) : base(jsValue, value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value) => base.SetJsValue(value);

        public string GetCreationCode() => TypedValue.ToString();

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
            => builder.RequestBasicObjectCreation(TypedValue);
    }
}
