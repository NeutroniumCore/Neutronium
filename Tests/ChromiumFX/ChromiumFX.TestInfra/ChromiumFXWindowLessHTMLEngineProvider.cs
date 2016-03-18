using System;
using Chromium;
using HTMEngine.ChromiumFX;
using IntegratedTest.Infra.Window;
using IntegratedTest.Infra.Windowless;
using KnockoutUIFramework.Test.IntegratedInfra;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace ChromiumFX.TestInfra 
{
    public class ChromiumFXWindowLessHTMLEngineProvider: IWindowLessHTMLEngineProvider 
    {
        private bool _Runing = false;
        private readonly WpfThread _WpfThread;
        private ChromiumFXWPFWebWindowFactory _ChromiumFXWPFWebWindowFactory;

        public ChromiumFXWindowLessHTMLEngineProvider() 
        {
            _WpfThread = WpfThread.GetWpfThread();
            _WpfThread.AddRef();
        }

        public void Dispose() 
        {
            _WpfThread.Dispose();
        }

        private void Init() 
        {
            if (_Runing)
                return;        

            _Runing = true;
            _WpfThread.Dispatcher.Invoke(Initialize);
            _WpfThread.OnThreadEnded += (o, e) => _ChromiumFXWPFWebWindowFactory.Dispose();
        }

        private void Initialize() 
        {
            Func<CfxSettings> settings = () => new CfxSettings {
                SingleProcess = true,
                WindowlessRenderingEnabled = true,
                RemoteDebuggingPort = 9090
            };

            _ChromiumFXWPFWebWindowFactory = new ChromiumFXWPFWebWindowFactory(settings);
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
            return new ChromiumFXWindowlessJavascriptEngine(_WpfThread, frameWork);
        }
    }
}
