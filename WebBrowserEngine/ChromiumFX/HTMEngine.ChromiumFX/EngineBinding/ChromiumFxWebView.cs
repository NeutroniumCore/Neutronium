using System;
using System.Threading.Tasks;
using Chromium.Remote;
using Neutronium.Core;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.WebBrowserEngine.ChromiumFx.Convertion;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    internal class ChromiumFxWebView : IWebView
    {
        private readonly CfrFrame _CfrFrame;
        private readonly ChromiumFxDispatcher _Dispatcher;
        private readonly IWebSessionLogger _Logger;

        public IJavascriptObjectConverter Converter { get; }
        public IJavascriptObjectFactory Factory { get; }
        private CfrV8Context V8Context { get; }
        public bool AllowBulkCreation => true;
        public int MaxFunctionArgumentsNumber => 240000;

        public ChromiumFxWebView(CfrBrowser cfrbrowser, IWebSessionLogger logger) 
        {
            _Logger = logger;
            var browser = cfrbrowser;
            _CfrFrame = browser.MainFrame;
            V8Context = _CfrFrame.V8Context;
            _Dispatcher = new ChromiumFxDispatcher(browser, V8Context, _Logger);
            Converter = new ChromiumFxConverter(V8Context);
            Factory = new ChromiumFxFactory(V8Context, this);
        }

        internal CfrFrame GetRaw()
        {
            return _CfrFrame;
        }

        public Task RunAsync(Action act)
        {
           return _Dispatcher.RunAsync(act);
        }

        public void Dispatch(Action act)
        {
            _Dispatcher.Dispatch(act);
        }

        public void Run(Action act)
        {
            _Dispatcher.Run(act);
        }

        public Task<T> EvaluateAsync<T>(Func<T> compute)
        {
            return _Dispatcher.EvaluateAsync(compute);
        }

        public bool IsInContext() 
        {
            return _Dispatcher.IsInContext();
        }

        public T Evaluate<T>(Func<T> compute)
        {
            return _Dispatcher.Evaluate(compute);
        }

        public IJavascriptObject GetGlobal()
        {
            return V8Context.Global.Convert();
        }

        public bool Eval(string code, out IJavascriptObject res)
        {
            CfrV8Value v8Res;
            CfrV8Exception exception;
            var resValue = V8Context.Eval(code, string.Empty, 1, out v8Res, out exception);
            res = v8Res?.Convert();
            if (exception != null)
                _Logger.Error($"Error during javascript code evaluation. Code: {code}, Exception: {exception.Message}");
            return resValue;
        }

        public void ExecuteJavaScript(string code) 
        {
            Dispatch(() => _CfrFrame.ExecuteJavaScript(code, String.Empty, 0));
        }
    }
}
