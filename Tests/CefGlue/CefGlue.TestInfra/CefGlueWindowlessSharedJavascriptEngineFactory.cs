using IntegratedTest;
using IntegratedTest.Windowless.Infra;
using KnockoutUIFramework.Test.IntegratedInfra;
using MVVM.Cef.Glue.CefSession;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace CefGlue.TestInfra
{
    public class CefGlueWindowlessSharedJavascriptEngineFactory : IWindowLessHTMLEngineProvider
    {
        private bool _Runing=false;
        private readonly FakeUIThread _FakeUIThread;

        public CefGlueWindowlessSharedJavascriptEngineFactory() 
        {
            _FakeUIThread = new FakeUIThread( () => CefCoreSessionSingleton.GetAndInitIfNeeded(), CefCoreSessionSingleton.Clean );
        }

        private void Init() 
        {
            if (_Runing)
                return;

            _Runing = true;
            _FakeUIThread.Start();
        }

        public void Dispose()
        {
            _FakeUIThread.Stop();
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
