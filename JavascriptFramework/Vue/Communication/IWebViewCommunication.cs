using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;

namespace Neutronium.JavascriptFramework.Vue.Communication
{
    public interface IWebViewCommunication
    {
        void RegisterCommunicator(IWebView webView);

        IDisposable Subscribe(IWebView webView, string channel, Action<string> onEvent);

        IDisposable ExecuteCodeOnEvent(IWebView source, string channel, IWebView target, Func<string, string> codeBuilder);
    }
}
