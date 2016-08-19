using System;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace MVVM.HTML.Core.Infra
{
    public class NullLogger : IWebSessionLogger
    {
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
    }
}
