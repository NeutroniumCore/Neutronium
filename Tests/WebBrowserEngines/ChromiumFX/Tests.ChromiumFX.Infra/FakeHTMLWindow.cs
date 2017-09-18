using System;
using Chromium;
using Chromium.Event;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding;

namespace Tests.ChromiumFX.Infra 
{
    internal class FakeHTMLWindow : IWebBrowserWindow
    {
        private readonly IWebView _Webview;
        private Uri _Uri;
        private readonly CfxClient _CfxClient;

        public Uri Url => _Uri;
        public bool IsLoaded => true;
        public IWebView MainFrame =>  _Webview; 

        public FakeHTMLWindow(CfxClient cfxClient, IWebView webview, Uri url) 
        {
            _Uri = url;
            _Webview = webview;
            _CfxClient = cfxClient;
            _CfxClient.GetDisplayHandler += OnGetDisplayHandler;
        }

        private void OnGetDisplayHandler(object sender, CfxGetDisplayHandlerEventArgs e)
        {
            var displayhandler = new CfxDisplayHandler();
            displayhandler.OnConsoleMessage += OnConsoleMessage;
            e.SetReturnValue(displayhandler);
        }

        private void OnConsoleMessage(object sender, CfxOnConsoleMessageEventArgs e)
        {
            ConsoleMessage?.Invoke(this, new ConsoleMessageArgs(e.Message, e.Source, e.Line));
        }

        public void NavigateTo(Uri path) 
        {
            throw new NotImplementedException();
        }

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage;

        public event EventHandler<LoadEndEventArgs> LoadEnd  { add { } remove { } }

        public event EventHandler<BrowserCrashedArgs> Crashed { add { } remove { } }
    }
}
