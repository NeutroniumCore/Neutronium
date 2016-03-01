using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace IntegratedTest
{
    public abstract class MVVMCefCore_Test_Base : IDisposable
    {
        protected IWebView _WebView = null;
        protected HTMLViewEngine _ICefGlueWindow = null;
        private IWindowlessJavascriptEngine _Tester;
        private IJavascriptFrameworkExtractor _JavascriptFrameworkExtractor;
        private WindowlessTestEnvironment _TestEnvironment;

        protected IJavascriptObjectConverter Converter { get { return _WebView.Converter; } }

        protected IJavascriptObjectFactory Factory { get { return _WebView.Factory; } }
        

        public MVVMCefCore_Test_Base(WindowlessTestEnvironment testEnvironment)
        {
            _TestEnvironment = testEnvironment;
        }

        protected virtual void Init()
        {
        }

        protected void Test(Action act, string ipath=null)
        {
            using (var disp = Tester(ipath))
            {
                Init();
                DoSafe(act);
            }
        }


        public IDisposable Tester(string ihtlmpath = null)
        {
            if (_Tester != null)
                _Tester.Dispose();

            _Tester = _TestEnvironment.Build();
            _Tester.Init(ihtlmpath);
            _ICefGlueWindow = _Tester.ViewEngine;
            _WebView = _Tester.WebView;
            _JavascriptFrameworkExtractor = _TestEnvironment.JavascriptFrameworkExtractorBuilder(_WebView);
            return _Tester;
        }

        public IDispatcher  GetTestUIDispacther()
        {
            return _TestEnvironment.TestUIDispacther;
        }

        protected T GetSafe<T>(Func<T> UnsafeGet)
        {
            return _WebView.EvaluateAsync(UnsafeGet).Result;
        }

        internal Task RunInContext(Action act)
        {
            return _WebView.RunAsync(act);
        }

        internal Task DispatchInContext(Action act)
        {
            return _WebView.DispatchAsync(act);
        }

        protected async Task RunAsync(TestInContext test)
        {
            using (Tester(test.Path))
            {
                using (var mb = await test.Bind(_ICefGlueWindow))
                {
                    if (test.Test!=null)
                        await RunInContext(() => test.Test(mb));
                    else
                        await RunInContext(async () => await test.TestAsync(mb));

                    if (test.Then!=null)
                    {
                        Thread.Sleep(200);
                        await DispatchInContext(() => test.Then(mb));
                    }
                }
            }
        }

        protected IJavascriptObject GetAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetAttribute(value, attibutename);
        }

        protected string GetStringAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetStringAttribute(value, attibutename);
        }

        protected int GetIntAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetIntAttribute(value, attibutename);
        }

        protected double GetDoubleAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetDoubleAttribute(value, attibutename);
        }

        protected bool GetBoolAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetBoolAttribute(value, attibutename);
        }

        protected IJavascriptObject CallWithRes(IJavascriptObject value, string functionname, params IJavascriptObject[] parameter)
        {
            return _WebView.Evaluate(() => value.Invoke(functionname, _WebView, parameter));
        }

        protected void Call(IJavascriptObject value, string functionname, params IJavascriptObject[] parameter)
        {
            _WebView.Run(() => value.Invoke(functionname,_WebView,parameter));
        }

        protected void Call(IJavascriptObject value, string functionname, Func<IEnumerable<IJavascriptObject>> parameter)
        {
            _WebView.Run(() => value.Invoke(functionname, _WebView, parameter().ToArray()));
        }

        protected void Call(IJavascriptObject value, string functionname, Func<IJavascriptObject> parameter)
        {
            _WebView.Run(() => value.Invoke(functionname, _WebView, parameter()));
        }

        protected void DoSafe(Action Doact)
        {
            _WebView.Run(Doact);
        }
    
        public void Dispose()
        {
            if (_Tester != null)
                _Tester.Dispose();

            _Tester = null;
        }
    }
}
