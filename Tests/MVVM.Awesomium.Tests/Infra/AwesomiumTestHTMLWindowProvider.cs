using MVVM.HTML.Core.JavascriptEngine.Control;
using MVVM.HTML.Core.JavascriptEngine.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IntegratedTest.Windowless;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.Awesomium.Tests
{
    internal class AwesomiumTestHTMLWindowProvider : IHTMLWindowProvider
    {
        internal AwesomiumTestHTMLWindowProvider(IWebView webView, string path)
        {
            HTMLWindow = new AwesomiumTestHTMLWindow(webView, path);
        }
        public IHTMLWindow HTMLWindow
        {
            get; private set;
        }

        public IDispatcher UIDispatcher
        {
            get { return new TestIUIDispatcher(); }
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
