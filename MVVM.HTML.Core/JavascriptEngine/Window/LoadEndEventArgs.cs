using System;
using MVVM.HTML.Core.V8JavascriptObject;


namespace MVVM.HTML.Core.Window
{
	public class LoadEndEventArgs : EventArgs
	{
        public LoadEndEventArgs(IWebView frame)
		{
			Frame = frame;
		}

        public IWebView Frame { get; private set; }
	}
}
