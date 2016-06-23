using System;

namespace MVVM.HTML.Core.Navigation
{
    public class NullWatcher : IWebSessionWatcher
    {
        void IWebSessionWatcher.LogCritical(string iInformation)
        {
        }

        void IWebSessionWatcher.LogBrowser(string iInformation)
        {
        }

        void IWebSessionWatcher.OnSessionError(Exception exception, Action cancel)
        {
        }
    }
}
