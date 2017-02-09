using System.Collections.Generic;
using System.Linq;
using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.V8Object;

namespace Neutronium.WebBrowserEngine.ChromiumFx.Convertion 
{
    public static class ChromiumFxJavascriptObjectExtension 
    {
        public static IJavascriptObject Convert(this CfrV8Value cfrV8Value) 
        {
            if ((cfrV8Value == null) || (!cfrV8Value.IsObject)) 
                return new ChromiumFXJavascriptSimple(cfrV8Value);

            return new ChromiumFXJavascriptObject(cfrV8Value); 
        }

        public static CfrV8Value Convert(this IJavascriptObject javascriptObject) 
        {
            var chromiumObject = javascriptObject as ICfxJavascriptObject;
            return chromiumObject?.GetRaw();
        }

        public static IJavascriptObject[] Convert(this IEnumerable<CfrV8Value> cfrV8Values) 
        {
            return cfrV8Values.Select(v8 => v8.Convert()).ToArray();
        }

        public static CfrV8Value[] Convert(this IEnumerable<IJavascriptObject> javascriptObjects) 
        {
            return javascriptObjects.Select(jo => jo.Convert()).ToArray();
        }
    }
}
