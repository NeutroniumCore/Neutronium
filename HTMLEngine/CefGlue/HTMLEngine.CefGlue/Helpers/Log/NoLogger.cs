using System;

namespace HTMLEngine.CefGlue.Helpers.Log
{
    public class NoLogger : ILogger 
    {
        public void TraceException(string message, Exception exception)
        {
        }

        public void Trace(string message, params object[] args)
        {
        }

        public void DebugException(string message, Exception exception)
        {
        }

        public void Debug(string message, params object[] args)
        {
        }

        public void ErrorException(string message, Exception exception)
        {
        }

        public void Error(string message, params object[] args)
        {
        }

        public void FatalException(string message, Exception exception)
        {
        }

        public void Fatal(string message, params object[] args)
        {
        }

        public void InfoException(string message, Exception exception)
        {
        }

        public void Info(string message, params object[] args)
        {
        }

        public void WarnException(string message, Exception exception)
        {
        }

        public void Warn(string message, params object[] args)
        {
        }

        public bool IsTraceEnabled
        {
            get { return false; }
        }

        public bool IsDebugEnabled
        {
            get { return false; }
        }

        public bool IsErrorEnabled
        {
            get { return false; }
        }

        public bool IsFatalEnabled
        {
            get { return false; }
        }

        public bool IsInfoEnabled
        {
            get { return false; }
        }

        public bool IsWarnEnabled
        {
            get { return false; }
        }
    }
}
