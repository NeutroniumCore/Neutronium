using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.JavascriptEngine.Window
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
