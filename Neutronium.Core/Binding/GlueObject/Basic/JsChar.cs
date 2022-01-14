using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    public sealed class JsChar : JsBasicTyped<char>, IBasicJsCsGlue
    {
        public JsChar(char value) : base(value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value, ISessionCache cache) => base.SetJsValue(value);

        public string GetCreationCode() => JavascriptNamer.GetCreateCharString(TypedValue);

        public override string ToString() => JavascriptNamer.GetCreateCharStringDoubleQuote(TypedValue);

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestStringCreation(GetCreationCode());
    }
}
