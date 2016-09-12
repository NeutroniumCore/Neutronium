using System;
using System.Runtime.CompilerServices;
using Neutronium.Core;
using Neutronium.Core.JavascriptEngine.Window;
using Xunit.Abstractions;

namespace Tests.Infra.HTMLEngineTesterHelper.Windowless
{
    public class TestLogger : IWebSessionLogger
    {
        private readonly ITestOutputHelper _Output;

        public TestLogger(ITestOutputHelper output) 
        {
            _Output = output;
        }

        private void Log(string message, [CallerMemberName] string memberName = "") 
        {
            try
            {
                _Output.WriteLine($"{memberName}: {message}");
            }
            catch(Exception)
            {
                //May happen if we try to log something after test complete
                //may happen if a thread use a refernce to an ended test
            }
        }

        private void Log(Func<string> message, [CallerMemberName] string memberName = "") 
        {
            Log(message(), memberName);
        }

        public void Debug(string information) 
        {
            Log(information);
        }

        public void Debug(Func<string> information)
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