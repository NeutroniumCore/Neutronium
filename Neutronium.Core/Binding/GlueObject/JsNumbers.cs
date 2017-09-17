using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject
{
    internal sealed class JsInt: JsNumber<int>, IJsCsGlue
    {
        public JsInt(int value) : base(value) { }

        public JsInt(IJavascriptObject jsValue, int value) : base(jsValue, value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value)
        {
            base.SetJsValue(value);
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestBasicObjectCreation(NumberValue);
        }
    }
}
