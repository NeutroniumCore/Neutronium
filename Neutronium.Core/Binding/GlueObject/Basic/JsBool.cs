using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal sealed class JsBool : JsBasicTyped<bool>, IBasicJsCsGlue
    {
        internal JsBool(bool value) : base(value) { }

        internal JsBool(IJavascriptObject jsValue, bool value) : base(jsValue, value) { }

        public string GetCreationCode() => TypedValue ? "true" : "false";

        public override string ToString() => GetCreationCode();

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestBoolCreation(TypedValue);

        void IJsCsGlue.SetJsValue(IJavascriptObject value, IJavascriptSessionCache cache) => base.SetJsValue(value);
    }
}
