using HTMLEngine.CefGlue.CefSession;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using IntegratedTest.Infra.Windowless;
using MVVM.HTML.Core.JavascriptUIFramework;
using IntegratedTest.JavascriptUIFramework;

namespace CefGlue.TestInfra
{
    public abstract class CefGlueWindowlessSharedJavascriptEngineFactory : IWindowLessHTMLEngineProvider
    {
        private bool _Runing=false;
        private readonly WpfThread _WpfThread;

        protected abstract FrameworkTestContext FrameworkTestContext { get; }

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
                FrameworkTestContext = FrameworkTestContext,
                TestUIDispacther = new WPFUIDispatcher(_WpfThread.Dispatcher)
            };
        }

        private IWindowlessJavascriptEngine CreateWindowlessJavascriptEngine(IJavascriptUIFrameworkManager frameWork) 
        {
            Init();
            return new CefGlueWindowlessSharedJavascriptEngine(frameWork);
        }
    }
}
