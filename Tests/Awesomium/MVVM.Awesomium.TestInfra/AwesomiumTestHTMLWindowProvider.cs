using System;
using HTML_WPF.Component;
using IntegratedTest.Infra.Window;
using IntegratedTest.Infra.Windowless;
using MVVM.Awesomium.Tests;
using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace MVVM.Awesomium.TestInfra
{
    internal class AwesomiumTestHTMLWindowProvider : IHTMLWindowProvider
    {
        internal AwesomiumTestHTMLWindowProvider(IWebView webView, Uri path)
        {
            HTMLWindow = new AwesomiumTestHTMLWindow(webView, path);
        }
        public IHTMLWindow HTMLWindow
        {
            get; private set;
        }

        public IDispatcher UIDispatcher
        {
            get { return new WPFUIDispatcher(WpfThread.GetWpfThread().Dispatcher); }
        }

        public void Show()
        {
        }

        public void Hide()
        {
        }

        public void Dispose()
        {
        }
    }
}
