using System;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Log
{
    public class NullLogger : IWebSessionLogger
    {

        void IWebSessionLogger.Debug(Func<string> information)
        {
        }

        void IWebSessionLogger.Debug(string information) 
        {          
        }

        void IWebSessionLogger.Info(string information) 
        {
        }

        public void Info(Func<string> information) 
        {
        }

        void IWebSessionLogger.Error(string information)
        {
        }

        public void Error(Func<string> information) 
        {
        }

        public void LogBrowser(ConsoleMessageArgs iInformation, Uri url) 
        {
        }

        void IWebSessionLogger.WebBrowserError(Exception exception, Action cancel)
        {
        }

        void IWebSessionLogger.Info(Func<string> information)
        {
        }

        void IWebSessionLogger.Error(Func<string> information)
        {
        }

        void IWebSessionLogger.LogBrowser(ConsoleMessageArgs iInformation, Uri url)
        {
        }
    }
}
