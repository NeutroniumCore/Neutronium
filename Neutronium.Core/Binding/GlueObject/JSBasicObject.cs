using System;
using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Infra;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JsBasicObject : IJsCsGlue
    {
        public IJavascriptObject JsValue { get; private set; }
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Basic;
        public uint JsId => 0;
        public IEnumerable<IJsCsGlue> Children => null;
        public bool Marked { get; set; } = true;

        internal JsBasicObject(object value)
        {
            CValue = value;
        }

        void IJsCsGlue.SetJsValue(IJavascriptObject value)
        {
            JsValue = value;
        }

        internal JsBasicObject(IJavascriptObject jsValue, object value)
        {
            CValue = value;
            JsValue = jsValue;
        }

        public void RequestBuildInstruction(IJavascriptObjectBuilder builder)
        {
            builder.RequestBasicObjectCreation(CValue);
        }

        public override string ToString()
        {
            if (CValue == null)
            {
                return "null";
            }

            var emumValue = CValue as Enum;
            if (emumValue != null)
            {
                return $"{{\"type\":\"{emumValue.GetType().Name}\",\"intValue\":{Convert.ToInt32(emumValue)},\"name\":\"{emumValue.ToString()}\",\"displayName\":\"{emumValue.GetDescription()}\"}}";
            }

            return JavascriptNamer.GetCreateExpression(CValue);
        }

        public void BuilString(DescriptionBuilder context)
        {
            context.Append(ToString());
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
