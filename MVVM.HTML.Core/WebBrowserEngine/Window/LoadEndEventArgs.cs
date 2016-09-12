using System;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.WebBrowserEngine.Window
{
	public class LoadEndEventArgs : EventArgs
	{
        public LoadEndEventArgs(IWebView frame)
		{
			Frame = frame;
		}

        public IWebView Frame { get; }
	}
}
