using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal sealed class JsUshort: JsBasicTyped<ushort>, IBasicJsCsGlue
    {
        public JsUshort(ushort value) : base(value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value, IJavascriptSessionCache cache) => base.SetJsValue(value);

        public string GetCreationCode() => TypedValue.ToString();

        public override string ToString() => TypedValue.ToString();

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
            => builder.RequestIntCreation(TypedValue);
    }
}
