using System;
using Chromium.Event;
using Chromium.Remote.Event;
using Chromium.WebBrowser;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace HTMEngine.ChromiumFX.EngineBinding
{
    public class ChromiumFxControlHTMLWindow : IHTMLModernWindow
    {
        private readonly ChromiumWebBrowser _ChromiumWebBrowser;

        public IWebView MainFrame { get; private set; }

        public Uri Url
        {
            get { return _ChromiumWebBrowser.Url; }
        }

        public bool IsLoaded 
        {
            get { return !_ChromiumWebBrowser.IsLoading; }
        }

        public ChromiumFxControlHTMLWindow(ChromiumWebBrowser chromiumWebBrowser)
        {
            _ChromiumWebBrowser = chromiumWebBrowser;
            _ChromiumWebBrowser.LoadHandler.OnLoadEnd += OnLoadEnd;
            _ChromiumWebBrowser.DisplayHandler.OnConsoleMessage += OnConsoleMessage;
            _ChromiumWebBrowser.OnV8ContextCreated += OnV8ContextCreated;
            //_ChromiumWebBrowser.BrowserCreated += OnBrowserCreated;
        }

        //private void OnBrowserCreated(object sender, BrowserCreatedEventArgs e)
        //{
        //    MainFrame = new ChromiumFXWebView(e.Browser.MainFrame);
        //}

        private void OnV8ContextCreated(object sender, CfrOnContextCreatedEventArgs e)
        {
            MainFrame = new ChromiumFXWebView(e.Browser.MainFrame);
            var beforeJavascriptExecuted = BeforeJavascriptExecuted;
            if (beforeJavascriptExecuted == null) 
                return;

            Action<string> excecute = (code) => e.Frame.ExecuteJavaScript(code, String.Empty, 0);
            beforeJavascriptExecuted(this, new BeforeJavascriptExcecutionArgs(excecute));
        }

        private void OnConsoleMessage(object sender, CfxOnConsoleMessageEventArgs e)
        {
            var consoleMessage = ConsoleMessage;
            if (consoleMessage != null)
                consoleMessage(this, new ConsoleMessageArgs(e.Message, e.Source, e.Line));
        }

        private void OnLoadEnd(object sender, CfxOnLoadEndEventArgs e)
        {
            var loadEnd = LoadEnd;
            if (loadEnd != null)
                loadEnd(this, new LoadEndEventArgs(MainFrame);
        }        
        
        public void NavigateTo(Uri path)
        {
            _ChromiumWebBrowser.LoadUrl(path.AbsolutePath);
        }

        public event EventHandler<LoadEndEventArgs> LoadEnd;

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage;        
        
        public event EventHandler<BeforeJavascriptExcecutionArgs> BeforeJavascriptExecuted;

        public event EventHandler<BrowserCrashedArgs> Crashed
        {
            add { }
            remove { }
        }
    }
}
