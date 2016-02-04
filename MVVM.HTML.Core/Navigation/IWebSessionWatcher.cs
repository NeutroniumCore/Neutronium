using System;

namespace MVVM.HTML.Core
{
    public interface IWebSessionWatcher
    {
        void LogCritical(string iInformation);

        void LogBrowser(string iInformation);

        void OnSessionError(Exception exception, Action cancel);
    }
}
