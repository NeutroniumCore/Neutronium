using System.Globalization;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    public sealed class JsSByte : JsBasicTyped<sbyte>, IBasicJsCsGlue
    {
        public JsSByte(sbyte value) : base(value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value, ISessionCache cache) => base.SetJsValue(value);

        public string GetCreationCode() => TypedValue.ToString(CultureInfo.InvariantCulture);

        public override string ToString() => TypedValue.ToString();

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestIntCreation(TypedValue);
    }
}
