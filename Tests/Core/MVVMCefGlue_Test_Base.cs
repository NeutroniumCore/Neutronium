using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using MVVM.Cef.Glue.CefGlueHelper;
using MVVM.Cef.Glue.CefSession;
using MVVM.Cef.Glue.Test.CefWindowless;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.V8JavascriptObject;
using Xilium.CefGlue;
using MVVM.HTML.Core.JavascriptEngine;

namespace MVVM.Cef.Glue.Test.Core
{
    public abstract class MVVMCefGlue_Test_Base : IDisposable
    {
        protected IWebView _WebView = null;
        protected HTMLViewEngine _ICefGlueWindow = null;

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

        private class TestContext : IDisposable
        {
            private readonly MVVMCefGlue_Test_Base _Father;
            
            public TestContext(MVVMCefGlue_Test_Base Father, string ipath)
            {
                _Father = Father;
                var cc = InitTask(ipath).Result;
                _Father._WebView = cc.HTMLWindow.MainFrame;
            }

            private Task<IHTMLWindowProvider> InitTask(string ipath)
            {
                TaskCompletionSource<IHTMLWindowProvider> tcs = new TaskCompletionSource<IHTMLWindowProvider>();
                Task.Run(() =>
                {
                    CefCoreSessionSingleton.GetAndInitIfNeeded();

                    CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
                    cefWindowInfo.SetAsWindowless(IntPtr.Zero, true);

                    // Settings for the browser window itself (e.g. enable JavaScript?).
                    var cefBrowserSettings = new CefBrowserSettings();

                    // Initialize some the cust interactions with the browser process.
                    var cefClient = new TestCefClient();  
                    
                    ipath = ipath ?? "javascript\\index.html";
                    string fullpath = string.Format("{0}\\{1}", Assembly.GetExecutingAssembly().GetPath(), ipath);

                    // Start up the browser instance.
                    CefBrowserHost.CreateBrowser(cefWindowInfo,cefClient, cefBrowserSettings, fullpath);
    
                    cefClient.GetLoadedBroserAsync().ContinueWith(t =>
                        {
                            var frame = t.Result.GetMainFrame();
                            var context = CefCoreSessionSingleton.Session.CefApp.GetContext(frame);
                            var htmlWindowProvider = new TestCefGlueHTMLWindowProvider(frame);
                            _Father._ICefGlueWindow = new HTMLViewEngine(
                                htmlWindowProvider,
                                new KnockoutSessionInjectorFactory()
                            );
                            tcs.SetResult(htmlWindowProvider);
                        }
                    );
                } );

                return tcs.Task;
            }

            void IDisposable.Dispose()
            {
                CefCoreSessionSingleton.Clean();
            }
        }

        private IDisposable _Tester;
        internal IDisposable Tester(string ihtlmpath = null)
        {
            if (_Tester != null)
                _Tester.Dispose();

            _Tester = new TestContext(this, ihtlmpath);
            return _Tester;
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
                    await RunInContext(() => test.Test(mb));

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
            return _WebView.Evaluate(() => CefV8_Helper.Convert(value).Invoke(attibutename, _WebView)).Convert();
        }

        protected string GetStringAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => CefV8_Helper.Convert(value).Invoke(attibutename, _WebView).GetStringValue());
        }

        protected int GetIntAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetIntValue());
        }

        protected double GetDoubleAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetDoubleValue());
        }

        protected bool GetBoolAttribute(IJavascriptObject value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetBoolValue());
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
