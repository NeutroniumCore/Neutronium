using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MVVM.HTML.Core.V8JavascriptObject;


namespace MVVM.HTML.Core.Window
{
	public class LoadEndEventArgs : EventArgs
	{
        public LoadEndEventArgs(IWebView frame, int httpStatusCode)
		{
			Frame = frame;
			HttpStatusCode = httpStatusCode;
		}

		public int HttpStatusCode { get; private set; }

        public IWebView Frame { get; private set; }
	}
}
