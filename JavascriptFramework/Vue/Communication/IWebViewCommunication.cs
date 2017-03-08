using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;

namespace Neutronium.JavascriptFramework.Vue.Communication
{
    public interface IWebViewCommunication
    {
        void RegisterCommunicator(IWebView webView);

        IDisposable Subscribe(IWebView webView, Action<string> onEvent);

        IDisposable Listen(IWebView source, IWebView target);
    }
}
