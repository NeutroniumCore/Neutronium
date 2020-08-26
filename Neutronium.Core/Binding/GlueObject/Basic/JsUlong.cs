using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal sealed class JsLong : JsBasicTyped<long>, IBasicJsCsGlue
    {
        public JsLong(long value) : base(value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value, ISessionCache cache) => base.SetJsValue(value);

        public string GetCreationCode() => TypedValue.ToString();

        public override string ToString() => TypedValue.ToString();

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestDoubleCreation(TypedValue);
    }
}
