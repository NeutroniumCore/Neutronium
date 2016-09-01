using HTMLEngine.CefGlue.CefSession;
using HTML_WPF.Component;
using Tests.Infra.HTMLEngineTesterHelper.Context;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;
using Tests.Infra.HTMLEngineTesterHelper.Window;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;

namespace CefGlue.TestInfra
{
    public class CefGlueWindowlessSharedJavascriptEngineFactory : IBasicWindowLessHTMLEngineProvider 
    {
        private bool _Runing=false;
        private readonly WpfThread _WpfThread;
        private readonly ITestHtmlProvider _HtmlProvider;

        public CefGlueWindowlessSharedJavascriptEngineFactory(ITestHtmlProvider htmlProvider)
        {
            _HtmlProvider = htmlProvider;
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

        public IWindowlessHTMLEngineBuilder GetWindowlessEnvironment() 
        {
            return new WindowlessIntegratedTestEnvironment() 
            {
                WindowlessJavascriptEngineBuilder = () => CreateWindowlessJavascriptEngine(),
                HtmlProvider = _HtmlProvider,
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
