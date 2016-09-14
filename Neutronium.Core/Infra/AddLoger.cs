using System;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Infra
{
    public class AddLoger : IWebSessionLogger
    {
        private readonly IWebSessionLogger _Logger1;
        private readonly IWebSessionLogger _Logger2;

        public AddLoger(IWebSessionLogger logger1, IWebSessionLogger logger2)
        {
            _Logger1 = logger1;
            _Logger2 = logger2;
        }

        private void Do(Action<IWebSessionLogger> doOnLogger)
        {
            doOnLogger(_Logger1);
            doOnLogger(_Logger2);
        }

        public void Debug(Func<string> information)
        {
            Do(l => l.Debug(information));
        }

        public void Debug(string information)
        {
            Do(l => l.Debug(information));
        }

        public void Error(Func<string> information)
        {
            Do(l => l.Error(information));
        }

        public void Error(string information)
        {
            Do(l => l.Error(information));
        }

        public void Info(Func<string> information)
        {
            Do(l => l.Info(information));
        }

        public void Info(string information)
        {
            Do(l => l.Info(information));
        }

        public void LogBrowser(ConsoleMessageArgs information, Uri url)
        {
            Do(l => l.LogBrowser(information, url));
        }

        public void WebBrowserError(Exception exception, Action cancel)
        {
            Do(l => l.WebBrowserError(exception, cancel));
        }
    }
}
