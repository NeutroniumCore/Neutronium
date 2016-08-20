using System;
using Awesomium.Core;
using Awesomium.Windows.Controls;
using HTMLEngine.Awesomium.HTMLEngine;
using HTMLEngine.Awesomium.Internal;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace HTMLEngine.Awesomium
{
    internal class AwesomiumHTMLWindow : IHTMLWindow, IDisposable
    {
        private readonly IWebView _WebControl;

        public bool IsLoaded => _WebControl.IsDocumentReady;
        public Uri Url => _WebControl.Source;

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

            if (Crashed != null)
                Crashed(this, new BrowserCrashedArgs());
        }

        private void _WebControl_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            ConsoleMessage?.Invoke(this, new ConsoleMessageArgs(e.Message, e.Source, e.LineNumber));
        }

        public MVVM.HTML.Core.JavascriptEngine.JavascriptObject.IWebView MainFrame
        {
            get;  private set;
        }

        public void NavigateTo(Uri path)
        {
            _WebControl.Source = path;
        }

        private void FireLoaded()
        {
            if (LoadEnd != null)
                LoadEnd(this, new LoadEndEventArgs(MainFrame));
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
