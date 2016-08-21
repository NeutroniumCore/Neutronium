using MVVM.HTML.Core.JavascriptEngine.Window;
using System;
using System.Diagnostics;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    internal class TestDisplayHandler : CefDisplayHandler
    {
        protected override bool OnConsoleMessage(CefBrowser browser, string message, string source, int line)
        {
            var consoleArgs = new ConsoleMessageArgs(message, source, line);
            if (ConsoleMessage==null)
            {
                Trace.WriteLine(consoleArgs);
            }
            ConsoleMessage?.Invoke(this, consoleArgs);
            return false;
        }

        public event EventHandler<ConsoleMessageArgs> ConsoleMessage;
    }
}
