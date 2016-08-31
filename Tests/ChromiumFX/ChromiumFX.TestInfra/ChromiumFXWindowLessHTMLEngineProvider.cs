using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Chromium;
using Chromium.Remote;
using ChromiumFX.TestInfra.Helper;
using HTMEngine.ChromiumFX.EngineBinding;
using MVVM.HTML.Core.Infra;
using MVVM.HTML.Core;
using Tests.Infra.HTMLEngineTesterHelper.Window;
using Tests.Infra.HTMLEngineTesterHelper.Windowless;
using Tests.Infra.IntegratedContextTesterHelper.Windowless;
using Tests.Infra.JavascriptEngineTesterHelper;

namespace ChromiumFX.TestInfra
{
    public abstract class ChromiumFXWindowLessHTMLEngineProvider: IWindowLessHTMLEngineProvider 
    {
        private bool _Runing = false;
        private readonly WpfThread _WpfThread;
        private RenderProcessHandler _RenderProcessHandler;
        private TaskCompletionSource<ChromiumFXWebView> _TaskContextCreatedEventArgs;
        private CfrApp _CfrApp;
        public IWebSessionLogger Logger { get; set; }

        protected ChromiumFXWindowLessHTMLEngineProvider() 
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

            var settings = new CfxSettings 
            {
                SingleProcess = false,
                BrowserSubprocessPath = path,
                WindowlessRenderingEnabled = true,
                MultiThreadedMessageLoop = true,
                NoSandbox = true,
                LocalesDirPath = System.IO.Path.GetFullPath(@"cef\Resources\locales"),
                ResourcesDirPath = System.IO.Path.GetFullPath(@"cef\Resources")
            };

            if (!CfxRuntime.Initialize(settings, app, RenderProcessStartup))
                throw new Exception("Failed to initialize CEF library.");

            Thread.Sleep(200);

        }

        internal int RenderProcessStartup() 
        {
            var remoteProcessId = CfxRemoteCallContext.CurrentContext.ProcessId;
            _CfrApp = new CfrApp();
            _RenderProcessHandler = new RenderProcessHandler(_CfrApp, remoteProcessId);
            _RenderProcessHandler.OnNewFrame += (e) =>
            {
                _TaskContextCreatedEventArgs.TrySetResult(new ChromiumFXWebView(e.Browser, Logger));
            };
            _CfrApp.GetRenderProcessHandler += (s, e) =>
            {
                try
                {
                    e.SetReturnValue(_RenderProcessHandler);
                }
                catch (Exception ex)
                {
                    Logger.Error($"Exception raised during GetRenderProcessHandler SetReturnValue {ex.Message}, loading task is {_TaskContextCreatedEventArgs.Task.Status}");
                    throw;
                }
            };
          
            return CfrRuntime.ExecuteProcess(_CfrApp);
        }

        public IWindowlessIntegratedContextBuilder GetWindowlessEnvironment() 
        {
            return new WindowlessIntegratedTestEnvironment() 
            {
                WindowlessJavascriptEngineBuilder = () => CreateWindowlessJavascriptEngine(),
                FrameworkTestContext = FrameworkTestContext,
                TestUIDispacther = new NullUIDispatcher()
            };
        }

        protected abstract FrameworkTestContext FrameworkTestContext { get; }

        private IWindowlessHTMLEngine CreateWindowlessJavascriptEngine() 
        {
            Init();
            _TaskContextCreatedEventArgs = new TaskCompletionSource<ChromiumFXWebView>();
            return new ChromiumFxWindowlessHtmlEngine(_WpfThread, _TaskContextCreatedEventArgs.Task);
        }
    }
}
