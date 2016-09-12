using System;
using Neutronium.Core.JavascriptEngine.Control;
using Neutronium.Core.JavascriptEngine.JavascriptObject;
using Neutronium.Core.JavascriptEngine.Window;
using Neutronium.WPF.Internal;
using Tests.Infra.HTMLEngineTesterHelper.Window;

namespace Tests.Awesomium.Infra
{
    internal class AwesomiumTestHTMLWindowProvider : IHTMLWindowProvider
    {
        public IHTMLWindow HTMLWindow { get; }
        public IDispatcher UIDispatcher => new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher);

        internal AwesomiumTestHTMLWindowProvider(IWebView webView, Uri path)
        {
            HTMLWindow = new AwesomiumTestHTMLWindow(webView, path);
        }

        public void Show()
        {
        }

        public void Hide()
        {
        }

        public bool OnDebugToolsRequest() => false;

        public void CloseDebugTools() 
        {
        }

        public void Dispose()
        {
        }
    }
}
