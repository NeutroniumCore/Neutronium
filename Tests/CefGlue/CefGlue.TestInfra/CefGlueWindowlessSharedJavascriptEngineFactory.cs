using IntegratedTest;
using IntegratedTest.Infra.Window;
using IntegratedTest.Infra.Windowless;
using KnockoutUIFramework.Test.IntegratedInfra;
using MVVM.Cef.Glue.CefSession;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace CefGlue.TestInfra
{
    public class CefGlueWindowlessSharedJavascriptEngineFactory : IWindowLessHTMLEngineProvider
    {
        private bool _Runing=false;
        private readonly WpfThread _WpfThread;

        public CefGlueWindowlessSharedJavascriptEngineFactory() 
        {
            _WpfThread = WpfThread.GetWpfThread();
            _WpfThread.AddRef();
        }

        private void Init() 
        {
            if (_Runing)
                return;

            _Runing = true;
            _WpfThread.Dispatcher.Invoke(() => CefCoreSessionSingleton.GetAndInitIfNeeded());
            _WpfThread.OnThreadEnded += (o,e) => CefCoreSessionSingleton.Clean();
        }

        public void Dispose()
        {
            _WpfThread.Release();
        }

        public WindowlessTestEnvironment GetWindowlessEnvironment() 
        {
            return new WindowlessTestEnvironment() 
            {
                WindowlessJavascriptEngineBuilder = (frameWork) => CreateWindowlessJavascriptEngine(frameWork),
                FrameworkTestContext = KnockoutFrameworkTestContext.GetKnockoutFrameworkTestContext(),
                TestUIDispacther = new NullUIDispatcher()
            };
        }

        private IWindowlessJavascriptEngine CreateWindowlessJavascriptEngine(IJavascriptUIFrameworkManager frameWork) 
        {
            Init();
            return new CefGlueWindowlessSharedJavascriptEngine(frameWork);
        }
    }
}
