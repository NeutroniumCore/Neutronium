using System;

namespace MVVM.HTML.Core.Navigation
{
    public class NullWatcher : IWebSessionWatcher
    {
        void IWebSessionWatcher.LogCritical(string information)
        {
        }

        void IWebSessionWatcher.LogBrowser(string information)
        {
        }

        void IWebSessionWatcher.OnSessionError(Exception exception, Action cancel)
        {
        }
    }
}
