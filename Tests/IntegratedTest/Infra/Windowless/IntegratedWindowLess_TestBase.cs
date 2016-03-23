using System;
using System.Threading.Tasks;
using IntegratedTest.Infra.Threading;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Xunit.Abstractions;

namespace IntegratedTest.Infra.Windowless
{
    public abstract class IntegratedWindowLess_TestBase 
    {
        protected IWebView _WebView = null;
        protected HTMLViewEngine _ICefGlueWindow = null;
        private IJavascriptFrameworkExtractor _JavascriptFrameworkExtractor;
        private WindowlessTestEnvironment _TestEnvironment;
        private readonly ITestOutputHelper _Output;

        protected IJavascriptObjectConverter Converter { get { return _WebView.Converter; } }

        protected IJavascriptObjectFactory Factory { get { return _WebView.Factory; } }

        protected IntegratedWindowLess_TestBase(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
        { 
            _Output = output;
            _TestEnvironment = testEnvironment.GetWindowlessEnvironment();
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
            var tester = _TestEnvironment.Build();
            tester.Init(ihtlmpath);
            _ICefGlueWindow = tester.ViewEngine;
            _WebView = tester.WebView;
            _JavascriptFrameworkExtractor = _TestEnvironment.GetExtractor(_WebView);
            return tester;
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

        internal async Task RunInContext(Func<Task> act) 
        {
            var taskFactory = new DispatcherTaskFactory(_WebView);
            await await taskFactory.StartNew(act);
        }

        protected async Task RunAsync(TestInContext test)
        {
            using (Tester(test.Path)) 
            {
                _Output.WriteLine("Begin Binding");
                using (var mb = await test.Bind(_ICefGlueWindow))
                {
                    _Output.WriteLine("End Binding");
                    _Output.WriteLine("Begin Test");
                    await RunInContext(() => test.Test(mb));
                    _Output.WriteLine("End Test");
                }
            }
         
        }

        protected async Task RunAsync(TestInContextAsync test) 
        {            
            using (Tester(test.Path)) 
            {
                _Output.WriteLine("Begin Binding");
                using (var mb = await test.Bind(_ICefGlueWindow)) 
                {
                    _Output.WriteLine("End Binding");
                    _Output.WriteLine("Begin Test");
                    await  RunInContext(async () => await test.Test(mb));
                    _Output.WriteLine("Ending test");
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

        protected void Call(IJavascriptObject value, string functionname, Func<IJavascriptObject> parameter)
        {
            _WebView.Run(() => value.Invoke(functionname, _WebView, parameter()));
        }

        protected void DoSafe(Action Doact)
        {
            _WebView.Run(Doact);
        }
    }
}
