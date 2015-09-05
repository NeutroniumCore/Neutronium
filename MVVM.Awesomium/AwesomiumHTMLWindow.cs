using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

using Awesomium.Core;
using Awesomium.Windows.Controls;
using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.Window;


namespace MVVM.Awesomium
{
    internal class AwesomiumHTMLWindow : IHTMLWindow, IDisposable
    {
        private WebSession _Session;
        private IWebView _WebControl;

        public AwesomiumHTMLWindow(WebSession iSession, WebControl iWebControl)
        {
            _Session = iSession;
            _WebControl = iWebControl;
            _WebControl.SynchronousMessageTimeout = 0;
            _WebControl.ExecuteWhenReady(FireLoaded);
            _WebControl.ConsoleMessage += _WebControl_ConsoleMessage;

            MainFrame = new AwesomiumWebView(_WebControl);
        }

        private void _WebControl_ConsoleMessage(object sender, ConsoleMessageEventArgs e)
        {
            if (ConsoleMessage != null)
                ConsoleMessage(this, new ConsoleMessageArgs(e.Message, e.Source, e.LineNumber));
        }

        public   HTML.Core.V8JavascriptObject.IWebView MainFrame
        {
            get;  private set;
        }

        public void NavigateTo(string path)
        {
            _WebControl.Source = new Uri( path);
        }

        public bool IsLoaded
        {
            get { return _WebControl.IsDocumentReady; }
        }

        private void FireLoaded()
        {
            if (LoadEnd != null)
                LoadEnd(this, new LoadEndEventArgs(MainFrame));
        }

        public event EventHandler<LoadEndEventArgs> LoadEnd;

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage;

        public IDispatcher GetDispatcher()
        {
            return new AwesomiumDispatcher();
        }



        public void Dispose()
        {
            _WebControl.ConsoleMessage -= _WebControl_ConsoleMessage;
        }
    }
}
