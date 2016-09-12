using System;
using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.JavascriptEngine.Window
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
