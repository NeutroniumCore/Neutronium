using System.Globalization;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    public sealed class JsDouble : JsBasicTyped<double>, IBasicJsCsGlue
    {
        public JsDouble(double value) : base(value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value, ISessionCache cache) => base.SetJsValue(value);

        public string GetCreationCode() => JavascriptNamer.GetCreateDoubleString(TypedValue);

        public override string ToString() => TypedValue.ToString(CultureInfo.InvariantCulture);

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestDoubleCreation(TypedValue);
    }
}
