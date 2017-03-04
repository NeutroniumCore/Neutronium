using System;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    public class ConsoleMessageArgs : EventArgs
    {
        public string Message { get; }
        public string Source { get; }
        public int Line { get; }

        public ConsoleMessageArgs(string message, string source, int line)
        {
            Message = message;
            Source = source;
            Line = line;
        }

        public override string ToString() 
        {
            return $"Message: {Message}, source: {Source}, line number: {Line}";
        }
    }
}
