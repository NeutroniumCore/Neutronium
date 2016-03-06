using System;
using Xilium.CefGlue;

namespace CefGlue.TestInfra.CefWindowless
{
    internal class TestDisplayHandler : CefDisplayHandler
    {
        protected override bool OnConsoleMessage(CefBrowser browser, string message, string source, int line)
        {
            Console.WriteLine("Cef console message: {0} src: {1} line: {2}", message, source, line);
            return false;
        }
    }
}
