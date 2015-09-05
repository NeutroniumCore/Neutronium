using Awesomium.Core;
using MVVM.Awesomium.HTMLEngine;
using MVVM.HTML.Core.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Awesomium
{
    class AwesomiumWebView : HTML.Core.V8JavascriptObject.IWebView
    {
        private IWebView _IWebView;
        private IDispatcher _Dispatcher;
        private AwesomiumJavascriptObjectConverter _AwesomiumJavascriptObjectConverter;
        private AwesomiumJavascriptObjectFactory _AwesomiumJavascriptObjectFactory;
        public AwesomiumWebView(IWebView iwebview)
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

        public HTML.Core.V8JavascriptObject.IJavascriptObject GetGlobal()
        {
            return _IWebView.ExecuteJavascriptWithResult("window").Convert();
        }

        public HTML.Core.V8JavascriptObject.IJavascriptObjectConverter Converter
        {
            get { return _AwesomiumJavascriptObjectConverter; }
        }

        public HTML.Core.V8JavascriptObject.IJavascriptObjectFactory Factory
        {
            get { return _AwesomiumJavascriptObjectFactory; }
        }

        public bool Eval(string code, out HTML.Core.V8JavascriptObject.IJavascriptObject res)
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
