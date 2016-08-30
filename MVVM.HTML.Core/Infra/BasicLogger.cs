using System;
using System.Diagnostics;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace MVVM.HTML.Core.Infra
{
    public class BasicLogger : IWebSessionLogger
    {
        private const string _Header = "MVVM for CEFGlue";

        private void Log(string message) 
        {
            Trace.WriteLine($"{_Header} - {message}");
        }

        public void Debug(string information) 
        {
        }

        public void Debug(Func<string> information)
        {
        }

        public void Info(string information) 
        {
            Log($"Info: {information}");
        }

        public void Info(Func<string> information) 
        {
            Info(information());
        }

        public void Error(string information)
        {
            Log($"Error - {information}");
        }

        public void Error(Func<string> information) 
        {
            Error(information());
        }

        public void LogBrowser(ConsoleMessageArgs e, Uri url) 
        {
            Log($"Brower Log: {e}, page: {url}");
        }

        void IWebSessionLogger.WebBrowserError(Exception exception, Action cancel)
        {
            Log($"WebBrowser Error Exception raised: - {exception.Message}");
        }
    }
}
