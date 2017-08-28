using System;
using System.Threading.Tasks;
using Neutronium.Core;
using Neutronium.Core.Binding;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Tests.Infra.WebBrowserEngineTesterHelper.Context;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;
using Tests.Infra.WebBrowserEngineTesterHelper.Threading;
using Xunit.Abstractions;
using Neutronium.Core.Log;
using Neutronium.Core.WebBrowserEngine.Control;

namespace Tests.Infra.WebBrowserEngineTesterHelper.Windowless
{
    public abstract class TestBase 
    {
        protected IWebView _WebView;
        protected HtmlViewEngine _ViewEngine;
        protected IWebSessionLogger _Logger;
        protected readonly ITestOutputHelper _TestOutputHelper;
        protected IWebBrowserWindowProvider _WebBrowserWindowProvider;
        private readonly IWindowlessHTMLEngineBuilder _TestEnvironment;

        protected IJavascriptObjectConverter Converter => _WebView.Converter;
        protected IJavascriptObjectFactory Factory => _WebView.Factory;

        protected TestBase(IBasicWindowLessHTMLEngineProvider testEnvironment, ITestOutputHelper output) 
        {
            _TestEnvironment = testEnvironment.GetWindowlessEnvironment();
            _TestOutputHelper = output;
            var logger = new TestLogger(output);
            _Logger = logger.Add(new BasicLogger());
        }

        protected virtual void Init() 
        {
        }

        protected void Test(Action act, TestContext ipath = TestContext.Index) 
        {
            using (Tester(ipath)) 
            {
                Init();
                DoSafe(act);
            }
        }

        protected T Test<T>(Func<T> act, TestContext ipath = TestContext.Index)
        {
            using (Tester(ipath))
            {
                Init();
                return _WebView.Evaluate(act);
            }
        }

        protected async Task TestAsync(Func<Task> act, TestContext ipath = TestContext.Index)
        {
            using (Tester(ipath))
            {
                Init();
                await DoSafeAsync(act);
            }
        }

        protected virtual IWindowlessHTMLEngine Tester(TestContext context = TestContext.Index) 
        {
            var tester = _TestEnvironment.Build();
            var path = _TestEnvironment.HtmlProvider.GetHtmlPath(context);
            _Logger.Debug($"Loading file: {path}");
            tester.Init(path, _Logger);
            _WebView = tester.WebView;
            _WebBrowserWindowProvider = tester.HTMLWindowProvider;
            tester.HTMLWindow.ConsoleMessage += (_, e) => _Logger.LogBrowser(e, new Uri(path));
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

        protected internal Task RunInContext(Action act) 
        {
            return _WebView.RunAsync(act);
        }

        protected internal async Task RunInContext(Func<Task> act) 
        {
            var taskFactory = new DispatcherTaskFactory(_WebView);
            await await taskFactory.StartNew(act);
        }

        protected IJavascriptObject CallWithRes(IJavascriptObject value, string functionname, params IJavascriptObject[] parameter)
        {
            return _WebView.Evaluate(() => value.Invoke(functionname, _WebView, parameter));
        }

        protected void Call(IJavascriptObject value, string functionname, params IJavascriptObject[] parameter) 
        {
            _WebView.Run(() => value.Invoke(functionname, _WebView, parameter));
        }

        protected void Call(IJavascriptObject value, string functionname, Func<IJavascriptObject> parameter) 
        {
            _WebView.Run(() => value.Invoke(functionname, _WebView, parameter()));
        }

        protected IJavascriptObject Create(Func<IJavascriptObject> factory) 
        {
            return _WebView.Evaluate(factory);
        }

        protected void DoSafe(Action doact) 
        {
            _WebView.Run(doact);
        }

        protected async Task DoSafeAsync(Func<Task> doact)
        {
            await await _WebView.EvaluateAsync(doact);
        }
    }
}
