using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;

namespace Neutronium.JavascriptFramework.Vue.Communication
{
    public interface IWebViewCommunication
    {
        void RegisterCommunicator(IWebView webView);

        IDisposable Subscribe(IWebView webView, string channel, Action<string> onEvent);

        IDisposable SubscribeAll(IWebView webView, Action<string, string> onEvent);

        IDisposable ExecuteCodeOnAllEvents(IWebView source, IWebView target, Func<string, string, string> codeBuilder);

        IDisposable Connect(IWebView first, IWebView second);
    }
}
