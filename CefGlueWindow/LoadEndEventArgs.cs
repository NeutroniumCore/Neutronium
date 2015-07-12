using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xilium.CefGlue;

namespace CefGlue.Window
{
	public class LoadEndEventArgs : EventArgs
	{
		public LoadEndEventArgs(CefFrame frame, int httpStatusCode)
		{
			Frame = frame;
			HttpStatusCode = httpStatusCode;
		}

		public int HttpStatusCode { get; private set; }

		public CefFrame Frame { get; private set; }
	}
}
