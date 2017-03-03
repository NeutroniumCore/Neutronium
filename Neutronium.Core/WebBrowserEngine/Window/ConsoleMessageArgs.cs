using System;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    public class ConsoleMessageArgs : EventArgs
    {
        public string Message { get; }
        public string Source { get; }
        public int Line { get; }

        public ConsoleMessageArgs(string iMessage, string iSource, int iLine)
        {
            Message = iMessage;
            Source = iSource;
            Line = iLine;
        }

        public override string ToString() 
        {
            return $"Message: {Message}, source: {Source}, line number: {Line}";
        }
    }
}
