using System.Collections.Generic;
using System.Linq;
using Chromium.Remote;
using HTMEngine.ChromiumFX.V8Object;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace HTMEngine.ChromiumFX.Convertion 
{
    public static class ChromiumFXJavascriptObjectExtension 
    {
        public static IJavascriptObject Convert(this CfrV8Value cfrV8Value) 
        {
            return new ChromiumFXJavascriptObject(cfrV8Value);
        }

        public static CfrV8Value Convert(this IJavascriptObject javascriptObject) 
        {
            var chromiumObject = javascriptObject as ChromiumFXJavascriptObject;
            return (chromiumObject != null) ? chromiumObject.GetRaw() : null;
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
