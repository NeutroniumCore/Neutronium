using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding.GlueObject
{
    internal class JSBasicObject : IJSCSGlue
    {
        internal JSBasicObject(IJavascriptObject value, object icValue)
        {
            JSValue = value;
            CValue = icValue;
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

        public IJavascriptObject JSValue { get; }

        public object CValue { get; }

        public JSCSGlueType Type => JSCSGlueType.Basic;

        public IEnumerable<IJSCSGlue> GetChildren()
        {
            return Enumerable.Empty<IJSCSGlue>();
        }
    }
}
