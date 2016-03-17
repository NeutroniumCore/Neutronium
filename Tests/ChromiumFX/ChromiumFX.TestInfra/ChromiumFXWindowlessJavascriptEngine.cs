using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Chromium.Remote;
using Chromium.WebBrowser;
using HTMEngine.ChromiumFX.EngineBinding;
using IntegratedTest.Infra.Window;
using IntegratedTest.Infra.Windowless;
using MVVM.HTML.Core.Binding;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptUIFramework;

namespace ChromiumFX.TestInfra
{
    public class ChromiumFXWindowlessJavascriptEngine : IWindowlessJavascriptEngine 
    {
        private readonly IJavascriptUIFrameworkManager _FrameWork;
        private readonly WpfThread _WpfThread;
        public HTMLViewEngine ViewEngine { get; private set;  }
        public IWebView WebView { get; private set; }

        public ChromiumFXWindowlessJavascriptEngine(WpfThread wpfThread, IJavascriptUIFrameworkManager frameWork) 
        {
            _FrameWork = frameWork;
            _WpfThread = wpfThread;
        }

        public void Init(string path = "javascript\\index.html") 
        {
            path = path ?? "https://www.google.com.br/";
            //"javascript\\index.html";
            InitAsync(path).Wait();
        }

        private async Task InitAsync(string path) 
        {
            var frame = await _WpfThread.Dispatcher.Invoke(() => RawInit(path));
            WebView = new ChromiumFXWebView(frame);
            ViewEngine = new HTMLViewEngine(new ChromiumFXHTMLWindowProvider(WebView, new Uri(path)), _FrameWork);
        }

        private async Task<CfrFrame> RawInit(string path) 
        {
            var tcs = new TaskCompletionSource<CfrFrame>();
            var tcsload = new TaskCompletionSource<int>();

            //int retval = CfxRemoting.ExecuteProcess();

            var wb = new ChromiumWebBrowser();

            var f = new Form {
                Size = new System.Drawing.Size(800, 600)
            };
            wb.Dock = DockStyle.Fill;
            wb.Parent = f;

            wb.OnV8ContextCreated += (sender, args) => tcs.SetResult(args.Frame);
            wb.LoadHandler.OnLoadEnd += (sender, args) => tcsload.TrySetResult(0);
            f.Show();
            wb.LoadUrl(path);

            System.Windows.Forms.Application.Run(f);

            //;
            await tcsload.Task;
            return await tcs.Task;
        }

        public void Dispose() 
        {
        }
    }
}
