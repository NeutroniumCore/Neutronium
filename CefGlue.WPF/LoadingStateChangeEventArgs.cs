using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xilium.CefGlue.WPF
{
    public class LoadingStateChangeEventArgs : EventArgs
    {
        public bool IsLoading { get; private set; }
        public bool CanGoBack { get; private set; }
        public bool CanGoForward { get; private set; }

        public LoadingStateChangeEventArgs(bool isLoading, bool canGoBack, bool canGoForward)
        {
            this.IsLoading = isLoading;
            this.CanGoBack = canGoBack;
            this.CanGoForward = canGoForward;
        }
    }
}
