using System;
using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object
{
    internal abstract class ChromiumFXJavascriptSimpleBase : ChromiumFXJavascriptRoot
    {
        internal ChromiumFXJavascriptSimpleBase(CfrV8Value cfrV8Value) : base(cfrV8Value)
        {
        }

        public void Bind(string functionName, IWebView context, Action<string, IJavascriptObject, IJavascriptObject[]> action) 
        {
            throw new NotImplementedException();
        }

        public IJavascriptObject ExecuteFunction(IWebView webView, IJavascriptObject context, params IJavascriptObject[] parameters)
        {
            throw new NotImplementedException();
        }

        public void ExecuteFunctionNoResult(IWebView webView, IJavascriptObject context, params IJavascriptObject[] parameters)
        {
            throw new NotImplementedException();
        }

        public IJavascriptObject ExecuteFunction(IWebView context) 
        {
            return null;
        }
    }
}
