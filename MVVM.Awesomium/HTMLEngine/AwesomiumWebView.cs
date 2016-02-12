using MVVM.Awesomium.HTMLEngine;
using MVVM.HTML.Core.Window;
using System;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using AwesomiumIWebView = Awesomium.Core.IWebView;
using MVVMIWebView = MVVM.HTML.Core.JavascriptEngine.JavascriptObject.IWebView;

namespace MVVM.Awesomium
{
    internal class AwesomiumWebView : MVVMIWebView
    {
        private AwesomiumIWebView _IWebView;
        private IDispatcher _Dispatcher;
        private AwesomiumJavascriptObjectConverter _AwesomiumJavascriptObjectConverter;
        private AwesomiumJavascriptObjectFactory _AwesomiumJavascriptObjectFactory;
        public AwesomiumWebView(AwesomiumIWebView iwebview)
        {
            _IWebView = iwebview;
            _Dispatcher = new AwesomiumDispatcher();
            _AwesomiumJavascriptObjectConverter = new AwesomiumJavascriptObjectConverter(_IWebView);
            _AwesomiumJavascriptObjectFactory = new AwesomiumJavascriptObjectFactory(_IWebView);
        }

        public Task DispatchAsync(Action act)
        {
           return _Dispatcher.RunAsync(act);
        }

        public IJavascriptObject GetGlobal()
        {
            return _IWebView.ExecuteJavascriptWithResult("window").Convert();
        }

        public IJavascriptObjectConverter Converter
        {
            get { return _AwesomiumJavascriptObjectConverter; }
        }

        public IJavascriptObjectFactory Factory
        {
            get { return _AwesomiumJavascriptObjectFactory; }
        }

        public bool Eval(string code, out IJavascriptObject res)
        {
           res = _IWebView.ExecuteJavascriptWithResult(code).Convert();
           return res != null;
        }

        public void ExecuteJavaScript(string code)
        {
            _IWebView.ExecuteJavascript(code);
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
    }
}
