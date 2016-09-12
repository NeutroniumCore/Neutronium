using System;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.WindowImplementation
{
    public class LoadStartEventArgs : EventArgs
    {
		public LoadStartEventArgs(CefFrame frame)
		{
			Frame = frame;
		}

		public CefFrame Frame { get; private set; }
    }
}
