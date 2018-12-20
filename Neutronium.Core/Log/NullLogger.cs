using System;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Log
{
    public struct NullLogger : IWebSessionLogger
    {
        public void Debug(Func<string> information)
        {
        }

        public void Debug(string information) 
        {          
        }

        public void Info(string information) 
        {
        }

        public void Info(Func<string> information) 
        {
        }

        public void Error(string information)
        {
        }

        public void Error(Func<string> information) 
        {
        }

        public void Warning(string information)
        {
        }

        public void Warning(Func<string> information)
        {
        }

        public void LogBrowser(ConsoleMessageArgs iInformation, Uri url) 
        {
        }

        public void WebBrowserError(Exception exception, Action cancel)
        {
        }
    }
}
