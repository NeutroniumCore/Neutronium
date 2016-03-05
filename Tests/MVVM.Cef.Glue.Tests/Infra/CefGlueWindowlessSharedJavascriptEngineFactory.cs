using System.Threading.Tasks;
using IntegratedTest;
using KnockoutUIFramework.Test.IntegratedInfra;
using MVVM.Cef.Glue.CefSession;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace MVVM.Cef.Glue.Tests.Infra
{
    public class CefGlueWindowlessSharedJavascriptEngineFactory : IWindowLessHTMLEngineProvider
    {
        private bool _Runing=false;

        private Task InitTask() 
        {
            if (_Runing)
                return TaskHelper.Ended();

            TaskCompletionSource<int> tcs = new TaskCompletionSource<int>();
            Task.Run(() =>
            {
                CefCoreSessionSingleton.GetAndInitIfNeeded();

                ////CefWindowInfo cefWindowInfo = CefWindowInfo.Create();
                ////cefWindowInfo.SetAsWindowless(IntPtr.Zero, true);

                ////// Settings for the browser window itself (e.g. enable JavaScript?).
                ////var cefBrowserSettings = new CefBrowserSettings();

                //// Initialize some the cust interactions with the browser process.
                //var cefClient = new TestCefClient();

                tcs.TrySetResult(0);

            });
            _Runing = true;

            return tcs.Task;
        }

        public void Dispose()
        {
            CefCoreSessionSingleton.Clean();
        }

        public WindowlessTestEnvironment GetWindowlessEnvironment() 
        {
            return new WindowlessTestEnvironment() {
                WindowlessJavascriptEngineBuilder = (frameWork) => CreateWindowlessJavascriptEngine(frameWork),
                FrameworkTestContext = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext(),
                TestUIDispacther = new NullUIDispatcher()
            };
        }

        private IWindowlessJavascriptEngine CreateWindowlessJavascriptEngine(IJavascriptUIFrameworkManager frameWork) 
        {
            InitTask().Wait();
            return new CefGlueWindowlessSharedJavascriptEngine(frameWork);
        }
    }
}
