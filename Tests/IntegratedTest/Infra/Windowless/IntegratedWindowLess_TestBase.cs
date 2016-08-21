using System;
using System.Threading.Tasks;
using IntegratedTest.Infra.Threading;
using IntegratedTest.JavascriptUIFramework;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Xunit.Abstractions;
using UIFrameworkTesterHelper;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Infra;

namespace IntegratedTest.Infra.Windowless
{
    public abstract class IntegratedWindowLess_TestBase 
    {
        protected IWebView _WebView = null;
        protected HTMLViewEngine _ViewEngine = null;
        private IJavascriptFrameworkExtractor _JavascriptFrameworkExtractor;
        private WindowlessTestEnvironment _TestEnvironment;
        protected IWebSessionLogger _Logger;

        protected IJavascriptObjectConverter Converter => _WebView.Converter;
        protected IJavascriptObjectFactory Factory => _WebView.Factory;

        protected IntegratedWindowLess_TestBase(IWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output)
        { 
            _TestEnvironment = testEnvironment.GetWindowlessEnvironment();
            var logger = new TestLogger(output);
            _Logger = logger.Add(new BasicLogger());
        }

        protected virtual void Init()
        {
        }

        protected void Test(Action act, TestContext ipath= TestContext.Index)
        {
            using (Tester(ipath))
            {
                Init();
                DoSafe(act);
            }
        }

        public IDisposable Tester(TestContext context = TestContext.Index)
        {
            var tester = _TestEnvironment.Build();
            var path = _TestEnvironment.HtmlProvider.GetHtlmPath(context);
            _Logger.Info($"Loading file: {path}");
            tester.Init(path, _Logger);
            _ViewEngine = tester.ViewEngine;
            _WebView = tester.WebView;
            tester.HTMLWindow.ConsoleMessage += (_,e) => _Logger.LogBrowser(e, new Uri(path));
            _JavascriptFrameworkExtractor = _TestEnvironment.GetExtractor(_WebView);
            return tester;
        }

        public IDispatcher GetTestUIDispacther()
        {
            return _TestEnvironment.TestUIDispacther;
        }

        protected T GetSafe<T>(Func<T> unsafeGet)
        {
            return _WebView.EvaluateAsync(unsafeGet).Result;
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
                _Logger.Info("Begin Binding");
                using (var mb = await test.Bind(_ViewEngine))
                {
                    _Logger.Info("End Binding");
                    _Logger.Info("Begin Test");
                    await RunInContext(() => test.Test(mb));
                    _Logger.Info("End Test");
                }
            }      
        }

        protected async Task RunAsync(TestInContextAsync test) 
        {            
            using (Tester(test.Path)) 
            {
                _Logger.Info("Begin Binding");
                using (var mb = await test.Bind(_ViewEngine)) 
                {
                    _Logger.Info("End Binding");
                    _Logger.Info("Begin Test");
                    await RunInContext(async () => await test.Test(mb));
                    _Logger.Info("Ending test");
                }
            }            
        }

        protected void SetAttribute(IJavascriptObject father, string attibutename, IJavascriptObject value)
        {
            _JavascriptFrameworkExtractor.SetAttribute(father, attibutename, value);
        }

        protected IJavascriptObject GetAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetAttribute(value, attibutename);
        }

        protected IJavascriptObject GetCollectionAttribute(IJavascriptObject value, string attibutename)
        {
            return _JavascriptFrameworkExtractor.GetCollectionAttribute(value, attibutename);
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

        protected IJavascriptObject Create(Func<IJavascriptObject> factory)
        {
            return _WebView.Evaluate(() => factory());
        }

        protected void DoSafe(Action Doact)
        {
            _WebView.Run(Doact);
        }
    }
}
