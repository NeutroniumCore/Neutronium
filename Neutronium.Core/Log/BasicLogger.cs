using System;
using System.Diagnostics;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Log
{
    public class BasicLogger : IWebSessionLogger
    {
        private const string _Header = "Neutronium";

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
            Trace.TraceInformation($"{_Header} - Info: {information}");
        }

        public void Info(Func<string> information) 
        {
            Info(information());
        }

        public void Error(string information)
        {
            Trace.TraceError($"{_Header} - Error: {information}");
        }

        public void Error(Func<string> information) 
        {
            Error(information());
        }

        public void LogBrowser(ConsoleMessageArgs e, Uri url) 
        {
            Log($"Browser Log: {e}, page: {url}");
        }

        void IWebSessionLogger.WebBrowserError(Exception exception, Action cancel)
        {
            Log($"WebBrowser Error Exception raised: - {exception.Message}");
        }
    }
}
