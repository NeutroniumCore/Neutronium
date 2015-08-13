using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xilium.CefGlue;

using MVVM.CEFGlue.Infra;
using MVVM.CEFGlue.CefSession;
using MVVM.CEFGlue.Test.CefWindowless;
using MVVM.CEFGlue.CefGlueHelper;
using MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject;


namespace MVVM.CEFGlue.Test
{
    public abstract class MVVMCefGlue_Test_Base : IDisposable
    {
        protected IWebView _WebView = null;
        protected TestCefGlueWindow _ICefGlueWindow = null;
        //private CefTaskRunner _CefTaskRunner;

        public MVVMCefGlue_Test_Base()
        {
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

        private class TestContext : IDisposable
        {
            private MVVMCefGlue_Test_Base _Father;
            
            public TestContext(MVVMCefGlue_Test_Base Father, string ipath)
            {
                _Father = Father;
                var cc = InitTask(ipath).Result;
                _Father._WebView = cc;
                //_Father._CefTaskRunner = cc.Runner;
                //CefRuntime.MessageLoopWork();
            }

            private Task<IWebView> InitTask(string ipath)
            {
                TaskCompletionSource<IWebView> tcs = new TaskCompletionSource<IWebView>();
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
                            _Father._ICefGlueWindow = new TestCefGlueWindow(frame);
                            tcs.SetResult(context);
                        }
                    );
                }
                );

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

        protected CefV8Value GetAttribute(CefV8Value value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView));
        }

        protected string GetStringAttribute(CefV8Value value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetStringValue());
        }

        protected int GetIntAttribute(CefV8Value value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetIntValue());
        }

        protected double GetDoubleAttribute(CefV8Value value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetDoubleValue());
        }

        protected bool GetBoolAttribute(CefV8Value value, string attibutename)
        {
            return _WebView.Evaluate(() => value.Invoke(attibutename, _WebView).GetBoolValue());
        }

        protected CefV8Value CallWithRes(CefV8Value value, string functionname, params CefV8Value[] parameter)
        {
            return _WebView.Evaluate(() => value.Invoke(functionname, _WebView, parameter));
        }

        protected void Call(CefV8Value value, string functionname, params CefV8Value[] parameter)
        {
            _WebView.Run(() => value.Invoke(functionname,_WebView,parameter));
        }

        protected void Call(CefV8Value value, string functionname, Func<IEnumerable<CefV8Value>> parameter)
        {
            _WebView.Run(() => value.Invoke(functionname, _WebView, parameter().ToArray()));
        }

        protected void Call(CefV8Value value, string functionname, Func<CefV8Value> parameter)
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
