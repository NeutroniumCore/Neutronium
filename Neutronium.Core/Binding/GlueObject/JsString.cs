using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    internal sealed class JsString : JsBasicTyped<string>, IBasicJsCsGlue
    {
        internal JsString(string value) : base(value) { }

        internal JsString(IJavascriptObject jsValue, string value) : base(jsValue, value) { }

        public string GetCreationCode() => JavascriptNamer.GetCreateExpression(TypedValue);

        public override string ToString() => GetCreationCode();

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestBasicObjectCreation(TypedValue);

        void IJsCsGlue.SetJsValue(IJavascriptObject value) => base.SetJsValue(value);
    }
}
