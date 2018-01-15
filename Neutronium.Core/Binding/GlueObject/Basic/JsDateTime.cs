using System;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal sealed class JsDateTime : JsBasicTyped<DateTime>, IBasicJsCsGlue
    {
        public JsDateTime(DateTime value) : base(value) { }

        void IJsCsGlue.SetJsValue(IJavascriptObject value, IJavascriptSessionCache cache) => base.SetJsValue(value);

        public string GetCreationCode() => JavascriptNamer.GetCreateDateTimeString(TypedValue);

        public override string ToString() => $@"d(""{TypedValue:yyyy-MM-ddTHH:mm:ss.FFFFFFF}"")";

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestJsDateTimeCreation(TypedValue);
    }
}
