using System;
using System.Threading.Tasks;
using Awesomium.Core;
using HTMLEngine.Awesomium.Internal;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptEngine.Window;
using AwesomiumIWebView = Awesomium.Core.IWebView;
using IWebView = Neutronium.Core.JavascriptEngine.JavascriptObject.IWebView;
using MVVMIWebView = Neutronium.Core.JavascriptEngine.JavascriptObject.IWebView;

namespace HTMLEngine.Awesomium.HTMLEngine
{
    internal class AwesomiumWebView : IWebView
    {
        private readonly AwesomiumIWebView _IWebView;
        private readonly IDispatcher _Dispatcher;
        private readonly AwesomiumJavascriptObjectConverter _AwesomiumJavascriptObjectConverter;
        private readonly AwesomiumJavascriptObjectFactory _AwesomiumJavascriptObjectFactory;
        private JSObject _Extracter;

        public AwesomiumWebView(AwesomiumIWebView iwebview)
        {
            _IWebView = iwebview;
            _Dispatcher = new AwesomiumDispatcher();
            _AwesomiumJavascriptObjectConverter = new AwesomiumJavascriptObjectConverter(_IWebView);
            _AwesomiumJavascriptObjectFactory = new AwesomiumJavascriptObjectFactory(_IWebView);
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

        public bool IsInContext() 
        {
            return _Dispatcher.IsInContext();
        }

        internal JSValue ExecuteFunction(JSValue function)
        {
            if (_Extracter == null)
            {
                _Extracter = _IWebView.ExecuteJavascriptWithResult("(function() { return { Extract : function(fn) { return fn(); } }; })()");
            }
            return _Extracter.Invoke("Extract", function);
        }
    }
}
