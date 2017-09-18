using System.Globalization;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal sealed class JsDecimal : JsBasicTyped<decimal>, IBasicJsCsGlue
    {
        public JsDecimal(decimal value) : base(value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value, IJavascriptSessionCache cache) => base.SetJsValue(value);

        public string GetCreationCode() => JavascriptNamer.GetCreateDoubleString((double)TypedValue);

        public override string ToString() => TypedValue.ToString(CultureInfo.InvariantCulture);

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestDoubleCreation((double)TypedValue);
    }
}
