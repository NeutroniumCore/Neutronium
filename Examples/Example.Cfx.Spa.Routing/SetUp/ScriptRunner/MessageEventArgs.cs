using System;

namespace Example.Cfx.Spa.Routing.SetUp.ScriptRunner
{
    public class MessageEventArgs : EventArgs
    {
        public string Message { get; }

        public bool Error { get; }

        public MessageEventArgs(string message, bool error = false)
        {
            Message = message;
            Error = error;
        }
    }
}