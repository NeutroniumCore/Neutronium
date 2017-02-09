using System;
using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object
{
    internal class ChromiumFXJavascriptSimple : ChromiumFXJavascriptBase, ICfxJavascriptObject
    {
        internal ChromiumFXJavascriptSimple(CfrV8Value cfrV8Value) : base(cfrV8Value)
        {
        }

        CfrV8Value ICfxJavascriptObject.GetRaw() 
        {
            return _CfrV8Value;
        }

        public void Bind(string functionName, IWebView context, Action<string, IJavascriptObject, IJavascriptObject[]> action) 
        {
            throw new NotImplementedException();
        }

        public IJavascriptObject ExecuteFunction(IWebView context) 
        {
            return null;
        }
    }
}
