using System.Collections.Generic;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;

namespace Neutronium.Core.Binding.GlueObject
{
    public static class JsCsGlueExtender
    {
        public static bool IsBasic(this IJsCsGlue @this)
        {
            return (@this.Type == JsCsGlueType.Basic);
        }

        public static void VisitDescendantsSafe(this IJsCsGlue @this, Func<IJsCsGlue, bool> visit)
        {
            var res = new HashSet<IJsCsGlue>();
            bool NewVisitor(IJsCsGlue glue) => (res.Add(glue)) && visit(glue);
            @this.VisitDescendants(NewVisitor);
        }

        public static IJavascriptObject GetJsSessionValue(this IJsCsGlue @this)
        {
            return (@this is IJsCsMappedBridge mappedBridge) ? mappedBridge.CachableJsValue : @this.JsValue;
        }
    }
}
