using System;
using System.Threading.Tasks;
using Chromium.Remote;
using HTMEngine.ChromiumFX.Convertion;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace HTMEngine.ChromiumFX.EngineBinding
{
    internal class ChromiumFXWebView : IWebView
    {
        private readonly CfrBrowser _Browser;
        private readonly CfrFrame _CfrFrame;
        private readonly ChromiumFXDispatcher _Dispatcher;    

        public IJavascriptObjectConverter Converter { get; private set; }
        public IJavascriptObjectFactory Factory { get; private set; }

        public ChromiumFXWebView(CfrBrowser cfrbrowser) 
        {
            _Browser = cfrbrowser;
            _CfrFrame = _Browser.MainFrame;
            V8Context = _CfrFrame.V8Context;
            _Dispatcher = new ChromiumFXDispatcher(_Browser, V8Context);
            Converter = new ChromiumFXConverter(V8Context);
            Factory = new ChromiumFXFactory(V8Context);
        }

        private CfrV8Context V8Context
        {
            set; get;
        }

        internal CfrFrame GetRaw()
        {
            return _CfrFrame;
        }

        public Task RunAsync(Action act)
        {
           return _Dispatcher.RunAsync(act);
        }

        public void Run(Action act)
        {
            _Dispatcher.Run(act);
        }

        public Task<T> EvaluateAsync<T>(Func<T> compute)
        {
            return _Dispatcher.EvaluateAsync(compute);
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
            res = null;
            CfrV8Value v8Res;
            CfrV8Exception exception;
            return V8Context.Eval(code, out v8Res, out exception);
        }

        public void ExecuteJavaScript(string code)
        {
            _CfrFrame.ExecuteJavaScript(code, String.Empty, 0);
        }
    }
}
