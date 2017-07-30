using System;
using System.Collections.Generic;
using System.Linq;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Infra;
using Neutronium.Core.Binding.Builder;
using Neutronium.Core.Binding.Listeners;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JSBasicObject : IJSCSGlue
    {
        public IJavascriptObject JSValue { get; private set; }
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Basic;
        public uint JsId => 0;

        internal JSBasicObject(object value)
        {
            CValue = value;
        }

        void IJSCSGlue.SetJSValue(IJavascriptObject value)
        {
            JSValue = value;
        }

        internal JSBasicObject(IJavascriptObject jsValue, object value)
        {
            CValue = value;
            JSValue = jsValue;
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
            if (CValue is string)
            {
                return $@"""{Normalize((string) CValue)}""";
            }

            object unBoxed = CValue;
            if (unBoxed is DateTime) 
            {
                var dt = (DateTime)unBoxed;
                return $@"""{dt.Year:0000}-{dt.Month:00}-{dt.Day:00}T{dt.Hour:00}:{dt.Minute:00}:{dt.Second:00}.{dt.Millisecond:000}Z""";
            }

            if (unBoxed is Enum)
            {
                var emumValue = (Enum)unBoxed;
                return $"{{\"type\":\"{emumValue.GetType().Name}\",\"intValue\":{Convert.ToInt32(emumValue)},\"name\":\"{emumValue.ToString()}\",\"displayName\":\"{emumValue.GetDescription()}\"}}";
            }

            if (unBoxed is bool)
            {
                var boolValue = (bool)unBoxed;
                return $"{(boolValue ? "true" : "false")}";
            }

            return CValue.ToString();
        }

        private static string Normalize(string value)
        {
            return value.Replace(@"\", @"\\").Replace(@"""", @"\""");
        }

        public void BuilString(DescriptionBuilder context)
        {
            context.Append(ToString());
        }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return Enumerable.Empty<IJSCSGlue>();
        }

        public void ApplyOnListenable(IObjectChangesListener listener)
        {
        }
    }
}
