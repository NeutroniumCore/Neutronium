using System;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    public class BeforeJavascriptExcecutionArgs : EventArgs
    {
        public BeforeJavascriptExcecutionArgs(Action<string> javascriptExecutor)
        {
            JavascriptExecutor = javascriptExecutor;
        }

        public Action<string> JavascriptExecutor { get; }
    }
}
