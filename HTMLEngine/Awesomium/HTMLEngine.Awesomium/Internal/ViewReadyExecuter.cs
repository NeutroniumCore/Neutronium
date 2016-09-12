using System;
using Awesomium.Core;

namespace Neutronium.WebBrowserEngine.Awesomium.Internal
{
    internal class ViewReadyExecuter
    {
        private readonly IWebView _WebView;
        private readonly Action _Do;
        private int _DoneCount = 0;
        internal ViewReadyExecuter(IWebView view, Action Do)
        {
            _WebView = view;
            _Do = Do;
        }

        internal void Do()
        {
            WebCore.QueueWork(() =>
            {
                if (_WebView.IsDocumentReady)
                    _DoneCount++;
                else
                    _WebView.DocumentReady += WebViewDocumentReady;

                if (_WebView.IsLoading)
                    _WebView.LoadingFrameComplete += WebViewLoadingFrameComplete;
                else
                    _DoneCount++;

                CheckCompletion(0);
            });
        }

        private void WebViewLoadingFrameComplete(object sender, FrameEventArgs e)
        {
            if (e.IsMainFrame)
            {
                _WebView.LoadingFrameComplete -= WebViewLoadingFrameComplete;
                CheckCompletion();
            }
        }

        private void WebViewDocumentReady(object sender, UrlEventArgs e)
        {
            _WebView.DocumentReady -= WebViewDocumentReady;
            CheckCompletion();
        }

        private void CheckCompletion(int add = 1)
        {
            _DoneCount += add;
            if (_DoneCount == 2)
                _Do();
        }
    }
}
