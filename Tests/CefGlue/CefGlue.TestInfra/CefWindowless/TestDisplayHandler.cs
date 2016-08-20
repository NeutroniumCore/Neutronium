using MVVM.HTML.Core.JavascriptEngine.Window;
using System;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    internal class TestDisplayHandler : CefDisplayHandler
    {
        protected override bool OnConsoleMessage(CefBrowser browser, string message, string source, int line)
        {
            ConsoleMessage?.Invoke(this, new ConsoleMessageArgs(message, source, line));
            return false;
        }

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage;
    }
}
