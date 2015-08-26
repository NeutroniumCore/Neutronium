using System;

namespace MVVM.Cef.Glue.WPF
{
    public interface IUiHelper
    {
        void PerformInUiThread(Action action);

        void StartAsynchronously(Action action);

        void PerformForMinimumTime(Action action, bool requiresUiThread, int minimumMillisecondsBeforeReturn);

        void IgnoreException(Action action);

        void Sleep(int milliseconds);

        void Sleep(TimeSpan sleepTime);
    }
}