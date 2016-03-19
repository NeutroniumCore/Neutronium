using System;
using System.IO;
using System.Threading.Tasks;
using Chromium;
using Chromium.Remote;
using ChromiumFX.TestInfra.Helper;
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
        private RenderProcessHandler _RenderProcessHandler;
        private TaskCompletionSource<Tuple<RenderProcessHandler,CfrApp>>  _TaskRenderProcessHandler;
        private CfrApp _CfrApp;

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
            _WpfThread.OnThreadEnded += (o, e) => CfxRuntime.Shutdown();
        }

        private void Initialize() 
        {
            CfxRuntime.LibCefDirPath = @"cef\Release";
            int retval = CfxRuntime.ExecuteProcess();

            var app = new CfxApp();
            var processHandler = new CfxBrowserProcessHandler();
            app.GetBrowserProcessHandler += (sender, e) => e.SetReturnValue(processHandler);

            var path = Path.Combine(GetType().Assembly.GetPath(), "ChromiumFXRenderProcess.exe");

            var settings = new CfxSettings {
                SingleProcess = false,
                BrowserSubprocessPath = path,
                WindowlessRenderingEnabled = true,
                RemoteDebuggingPort = 9090,
                MultiThreadedMessageLoop = true,
                NoSandbox = true,
                LocalesDirPath = System.IO.Path.GetFullPath(@"cef\Resources\locales"),
                ResourcesDirPath = System.IO.Path.GetFullPath(@"cef\Resources")
            };

            if (!CfxRuntime.Initialize(settings, app, RenderProcessStartup))
                throw new Exception("Failed to initialize CEF library.");

        }

        internal int RenderProcessStartup() 
        {
            var remoteProcessId = CfxRemoteCallContext.CurrentContext.ProcessId;
            _CfrApp = new CfrApp();
            _RenderProcessHandler = new RenderProcessHandler();
            _CfrApp.GetRenderProcessHandler += (s, e) => e.SetReturnValue(_RenderProcessHandler);
            _TaskRenderProcessHandler.TrySetResult(new Tuple<RenderProcessHandler, CfrApp>(_RenderProcessHandler, _CfrApp));
            return CfrRuntime.ExecuteProcess(_CfrApp);
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
            _TaskRenderProcessHandler = new TaskCompletionSource<Tuple<RenderProcessHandler, CfrApp>>();
            return new ChromiumFXWindowlessJavascriptEngine(_WpfThread, _TaskRenderProcessHandler.Task, frameWork);
        }
    }
}
