using System;
using System.Collections.Generic;
using Chromium;
using Chromium.Remote;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.Convertion;
using MoreCollection.Set;

namespace Neutronium.WebBrowserEngine.ChromiumFx.V8Object
{
    internal class  ChromiumFXJavascriptObject : ChromiumFXJavascriptBase, ICfxJavascriptObject
    {
        private readonly ISet<CfrV8Handler> _Functions = new HybridSet<CfrV8Handler>();

        internal ChromiumFXJavascriptObject(CfrV8Value cfrV8Value) :base(cfrV8Value)
        {
        }

        CfrV8Value ICfxJavascriptObject.GetRaw() 
        {
            return _CfrV8Value;
        }

        public override void Dispose() 
        {
            base.Dispose();
            _Functions.Clear();
        }

        public void Bind(string functionName, IWebView context, Action<string, IJavascriptObject, IJavascriptObject[]> action) 
        {
            lock (this) 
            {
                 var cfrV8Handler = action.Convert(functionName);
                _Functions.Add(cfrV8Handler);
                var func = CfrV8Value.CreateFunction(functionName, cfrV8Handler);
                _CfrV8Value.SetValue(functionName, func, CfxV8PropertyAttribute.ReadOnly | CfxV8PropertyAttribute.DontDelete);
            }           
        }

        public IJavascriptObject ExecuteFunction(IWebView context) 
        {
            return _CfrV8Value.ExecuteFunction(_CfrV8Value, new CfrV8Value[0]).Convert();
        }
    }
}
