using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    public sealed class JsUint : JsBasicTyped<uint>, IBasicJsCsGlue
    {
        public JsUint(uint value) : base(value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value, ISessionCache cache) => base.SetJsValue(value);

        public string GetCreationCode() => TypedValue.ToString();

        public override string ToString() => TypedValue.ToString();

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
            => builder.RequestUintCreation(TypedValue);
    }
}
