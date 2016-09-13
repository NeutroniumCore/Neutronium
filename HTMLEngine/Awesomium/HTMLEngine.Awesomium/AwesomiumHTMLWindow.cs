using System;
using Awesomium.Core;
using Awesomium.Windows.Controls;
using Neutronium.Core.WebBrowserEngine.Window;
using Neutronium.WebBrowserEngine.Awesomium.Engine;
using Neutronium.WebBrowserEngine.Awesomium.Internal;

namespace Neutronium.WebBrowserEngine.Awesomium
{
    internal class AwesomiumHTMLWindow : IWebBrowserWindow, IDisposable
    {
        private readonly IWebView _WebControl;

        public bool IsLoaded => _WebControl.IsDocumentReady;
        public Uri Url => _WebControl.Source;

        public Neutronium.Core.WebBrowserEngine.JavascriptObject.IWebView MainFrame 
        {
            get;
        }

        public AwesomiumHTMLWindow(WebControl iWebControl)
        {
            _WebControl = iWebControl;
            _WebControl.SynchronousMessageTimeout = 0;
            _WebControl.ExecuteWhenReady(FireLoaded);
            _WebControl.ConsoleMessage += _WebControl_ConsoleMessage;
            _WebControl.Crashed += _WebControl_Crashed;

            MainFrame = new AwesomiumWebView(_WebControl);
        }

        private void _WebControl_Crashed(object sender, CrashedEventArgs e)
        {
            if ((WebCore.IsShuttingDown) || (!WebCore.IsInitialized))
                return;

            Crashed?.Invoke(this, new BrowserCrashedArgs());
        }

        private void _WebControl_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            ConsoleMessage?.Invoke(this, new ConsoleMessageArgs(e.Message, e.Source, e.LineNumber));
        }

        public void NavigateTo(Uri path)
        {
            _WebControl.Source = path;
        }

        private void FireLoaded() 
        {
            LoadEnd?.Invoke(this, new LoadEndEventArgs(MainFrame));
        }

        public event EventHandler<LoadEndEventArgs> LoadEnd;

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage;

        public event EventHandler<BrowserCrashedArgs> Crashed;

        public void Dispose()
        {
            _WebControl.ConsoleMessage -= _WebControl_ConsoleMessage;
        }
    }
}
