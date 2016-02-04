using Awesomium.Core;
using System;

namespace MVVM.Awesomium
{
    internal class ViewReadyExecuter
    {
        private IWebView _IWebView;
        private Action _Do;
        private int _DoneCount = 0;
        internal ViewReadyExecuter(IWebView iview, Action Do)
        {
            _IWebView = iview;
            _Do = Do;
        }

        internal void Do()
        {
            WebCore.QueueWork(() =>
            {
                if (_IWebView.IsDocumentReady)
                    _DoneCount++;
                else
                    _IWebView.DocumentReady += _IWebView_DocumentReady;

                if (_IWebView.IsLoading)
                    _IWebView.LoadingFrameComplete += _IWebView_LoadingFrameComplete;
                else
                    _DoneCount++;

                CheckCompletion(0);
            });
        }

        void _IWebView_LoadingFrameComplete(object sender, FrameEventArgs e)
        {
            if (e.IsMainFrame)
            {
                _IWebView.LoadingFrameComplete -= _IWebView_LoadingFrameComplete;
                CheckCompletion();
            }
        }

        private void _IWebView_DocumentReady(object sender, UrlEventArgs e)
        {
            _IWebView.DocumentReady -= _IWebView_DocumentReady;
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
