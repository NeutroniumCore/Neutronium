using System;
using System.Collections.Generic;
using Chromium;
using Chromium.Remote;
using MoreCollection.Extensions;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.Convertion;
using MoreCollection.Set;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object
{
    internal abstract class ChromiumFXJavascriptObjectBase : ChromiumFXJavascriptRoot
    {
        private ISet<CfrV8Handler> _Functions;
        private ISet<CfrV8Handler> Functions => _Functions ?? (_Functions = new HybridSet<CfrV8Handler>());

        internal ChromiumFXJavascriptObjectBase(CfrV8Value cfrV8Value): base(cfrV8Value)
        {
        }

        public override void Dispose() 
        {
            base.Dispose();
            _Functions?.ForEach(f => f.Dispose());
            _Functions?.Clear();
        }

        public void Bind(string functionName, IWebView context, Action<string, IJavascriptObject, IJavascriptObject[]> action) 
        {
            lock (this) 
            {
                 var cfrV8Handler = action.Convert(functionName);
                Functions.Add(cfrV8Handler);
                var func = CfrV8Value.CreateFunction(functionName, cfrV8Handler);
                _CfrV8Value.SetValue(functionName, func, CfxV8PropertyAttribute.ReadOnly | CfxV8PropertyAttribute.DontDelete);
            }           
        }

        public IJavascriptObject ExecuteFunction(IWebView context) 
        {
            return _CfrV8Value.ExecuteFunction(_CfrV8Value, new CfrV8Value[0]).Convert();
        }

        public IJavascriptObject ExecuteFunction(IWebView webView, IJavascriptObject context, params IJavascriptObject[] parameters)
        {
            return _CfrV8Value.ExecuteFunction(context?.Convert(), parameters.Convert()).Convert();
        }

        public void ExecuteFunctionNoResult(IWebView webView, IJavascriptObject context, params IJavascriptObject[] parameters)
        {
            _CfrV8Value.ExecuteFunction(context?.Convert(), parameters.Convert());
        }
    }
}
