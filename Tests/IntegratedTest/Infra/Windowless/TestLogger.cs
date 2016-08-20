using System;
using System.Runtime.CompilerServices;
using MVVM.HTML.Core;
using MVVM.HTML.Core.JavascriptEngine.Window;
using Xunit.Abstractions;

namespace IntegratedTest.Infra.Windowless {
    internal class TestLogger : IWebSessionLogger
    {
        private readonly ITestOutputHelper _Output;

        public TestLogger(ITestOutputHelper output) 
        {
            _Output = output;
        }

        private void Log(string message, [CallerMemberName] string memberName = "") 
        {
            _Output.WriteLine($"{memberName}: {message}");
        }

        private void Log(Func<string> message, [CallerMemberName] string memberName = "") 
        {
            Log(message(), memberName);
        }

        public void Debug(string information) 
        {
            Log(information);
        }

        public void Info(string information) 
        {
            Log(information);
        }

        public void Info(Func<string> information) 
        {
            Log(information);
        }

        public void Error(string information) 
        {
            Log(information);
        }

        public void Error(Func<string> information) 
        {
            Log(information);
        }

        public void LogBrowser(ConsoleMessageArgs args, Uri url) 
        {
            Log($"{args}, page: {url}");
        }

        public void WebBrowserError(Exception exception, Action cancel) 
        {
            Log(exception.Message);
        }
    }
}