using System;
using System.Threading;
using Xilium.CefGlue.Helpers.Log;

namespace Xilium.CefGlue.WPF
{
    public class UiHelper : IUiHelper
    {
        private readonly ILogger _log;

        public UiHelper(ILogger log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _log = log;
        }

        public void PerformInUiThread(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var currentApplication = System.Windows.Application.Current;
            if ((currentApplication == null) || (Thread.CurrentThread == currentApplication.Dispatcher.Thread))
            {
                action.Invoke();
            }
            else
            {
                currentApplication.Dispatcher.Invoke(action);
            }
        }

        public void StartAsynchronously(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            (new Thread(action.Invoke)).Start();
        }

        public void PerformForMinimumTime(Action action, bool requiresUiThread, int minimumMillisecondsBeforeReturn)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var startTime = Environment.TickCount;

            if (requiresUiThread)
            {
                PerformInUiThread(action);
            }
            else
            {
                action.Invoke();
            }

            var remainingTime = minimumMillisecondsBeforeReturn - (Environment.TickCount - startTime);

            if (remainingTime > 0)
            {
                Thread.Sleep(remainingTime);
            }
        }

        public void IgnoreException(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                _log.ErrorException("Invocation failed", e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns>Default if fails</returns>
        public T IgnoreException<T>(Func<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            try
            {
                return action.Invoke();
            }
            catch (Exception e)
            {
                _log.ErrorException("Invokation failed", e);
                return default(T);
            }
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public void Sleep(TimeSpan sleepTime)
        {
            Thread.Sleep(sleepTime);
        }
    }
}