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

        public override string ToString() => $@"""{TypedValue.Year:0000}-{TypedValue.Month:00}-{TypedValue.Day:00}T{TypedValue.Hour:00}:{TypedValue.Minute:00}:{TypedValue.Second:00}.{TypedValue.Millisecond:000}Z""";

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestJsDateTimeCreation(TypedValue);
    }
}
