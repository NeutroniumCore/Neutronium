using System;
using System.Diagnostics;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace MVVM.HTML.Core.Infra
{
    public class BasicLogger : IWebSessionLogger
    {
        private const string _Header = "MVVM for CEFGlue";

        void IWebSessionLogger.Debug(string information) 
        {
            Info($"Debug - {information}");
        }

        public void Info(string information) 
        {
            Trace.WriteLine($"{_Header} - {information}");
        }

        public void Info(Func<string> information) 
        {
            Info(information());
        }

        public void Error(string information)
        {
            Info($"Error - {information}");
        }

        public void Error(Func<string> information) 
        {
            Error(information());
        }

        public void LogBrowser(ConsoleMessageArgs e, Uri url) 
        {
            Info($"Brower Log: {e}, page: {url}");
        }

        void IWebSessionLogger.WebBrowserError(Exception exception, Action cancel)
        {
            Info($"WebBrowser Error Exception raised: - {exception.Message}");
        }
    }
}
