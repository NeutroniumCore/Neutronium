﻿using Neutronium.Core.Binding.Builder;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.GlueObject.Basic
{
    public sealed class JsString : JsBasicGarbageCollectedTyped<string>, IBasicJsCsGlue
    {
        internal JsString(string value) : base(value) { }

        internal JsString(IJavascriptObject jsValue, string value) : base(jsValue, value) { }

        public string GetCreationCode() => JavascriptNamer.GetCreateExpression(TypedValue);

        public override string ToString() => JavascriptNamer.GetCreateExpressionDoubleQuote(TypedValue);

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder) => builder.RequestStringCreation(TypedValue);

        void IJsCsGlue.SetJsValue(IJavascriptObject value, ISessionCache cache) => base.SetJsValue(value);
    }
}
