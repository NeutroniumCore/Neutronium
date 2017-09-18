using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Infra;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    internal sealed class JsEnum : JsBasicTyped<Enum>, IBasicJsCsGlue
    {
        internal JsEnum(Enum value) : base(value) { }

        internal JsEnum(IJavascriptObject jsValue, Enum value) : base(jsValue, value) { }

        public string GetCreationCode() => $"new Enum('{TypedValue.GetType().Name}',{Convert.ToInt32(TypedValue)},'{TypedValue.ToString()}','{TypedValue.GetDescription()}')";

        public override string ToString() => $"{{\"type\":\"{TypedValue.GetType().Name}\",\"intValue\":{Convert.ToInt32(TypedValue)},\"name\":\"{TypedValue.ToString()}\",\"displayName\":\"{TypedValue.GetDescription()}\"}}";

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestEnumCreation(TypedValue);

        void IJsCsGlue.SetJsValue(IJavascriptObject value, IJavascriptSessionCache cache) => base.SetJsValue(value);
    }
}
