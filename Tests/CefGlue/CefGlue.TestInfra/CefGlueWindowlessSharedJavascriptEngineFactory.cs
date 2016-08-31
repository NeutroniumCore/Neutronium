using HTMLEngine.CefGlue.CefSession;
using HTML_WPF.Component;
using Tests.Infra.HTMLEngineTesterHelper.Window;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace CefGlue.TestInfra
{
    public abstract class CefGlueWindowlessSharedJavascriptEngineFactory : IWindowLessHTMLEngineProvider
    {
        private bool _Runing=false;
        private readonly WpfThread _WpfThread;

        protected abstract FrameworkTestContext FrameworkTestContext { get; }

        protected CefGlueWindowlessSharedJavascriptEngineFactory() 
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

        public IWindowlessIntegratedContextBuilder GetWindowlessEnvironment() 
        {
            return new WindowlessIntegratedTestEnvironment() 
            {
                WindowlessJavascriptEngineBuilder = () => CreateWindowlessJavascriptEngine(),
                FrameworkTestContext = FrameworkTestContext,
                TestUIDispacther = new WPFUIDispatcher(_WpfThread.Dispatcher)
            };
        }

        private IWindowlessHTMLEngine CreateWindowlessJavascriptEngine() 
        {
            Init();
            return new CefGlueWindowlessSharedHtmlEngine();
        }
    }
}
