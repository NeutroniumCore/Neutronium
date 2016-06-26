using System;

namespace MVVM.HTML.Core.JavascriptEngine.Window
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
