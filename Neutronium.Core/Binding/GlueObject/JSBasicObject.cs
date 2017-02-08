using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.Extension;
using Neutronium.Core.Exceptions;

namespace Neutronium.Core.Binding.GlueObject
{
    internal class JSBasicObject : IJSCSGlue
    {
        public IJavascriptObject JSValue { get; private set; }
        public object CValue { get; }
        public JsCsGlueType Type => JsCsGlueType.Basic;

        internal JSBasicObject(object value)
        {
            CValue = value;
        }

        internal JSBasicObject(IJavascriptObject jsValue, object value)
        {
            CValue = value;
            JSValue = jsValue;
        }

        public void ComputeJavascriptValue(IJavascriptObjectFactory factory, IJavascriptSessionCache cache)
        {
            IJavascriptObject value;
            if (factory.CreateBasic(CValue, out value))
            {
                JSValue = value;
                return;
            }
                
            if (!CValue.GetType().IsEnum)
                throw ExceptionHelper.Get("Algorithm core unexpected behaviour");

            JSValue = factory.CreateEnum((Enum)CValue);
            cache.CacheLocal(CValue, this);
        }

        public override string ToString()
        {
            if (CValue is string)
            {
                return $@"""{((string) CValue).Replace(@"""", @"\""")}""";
            }

            if (CValue is DateTime)
            {
                var dt = (DateTime)CValue;
                return $@"""{dt.Year:0000}-{dt.Month:00}-{dt.Day:00}T{dt.Hour:00}:{dt.Minute:00}:{dt.Second:00}.{dt.Millisecond:000}Z""";
            }

            if (CValue is Enum)
            {
                return $@"""{CValue.ToString()}""";
            }

            return CValue.ToString();
        }

        public void BuilString(StringBuilder sb, HashSet<IJSCSGlue> alreadyComputed)
        {
            sb.Append(this);
        }

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return Enumerable.Empty<IJSCSGlue>();
        }
    }
}
