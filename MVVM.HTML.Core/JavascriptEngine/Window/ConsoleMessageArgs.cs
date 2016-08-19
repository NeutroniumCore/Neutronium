using System;

namespace MVVM.HTML.Core.JavascriptEngine.Window
{
    public class ConsoleMessageArgs : EventArgs
    {
        public ConsoleMessageArgs(string iMessage, string iSource, int iLine)
        {
            Message = iMessage;
            Source = iSource;
            Line = iLine;
        }
        public string Message {get;set;}
        
        public string Source {get;set;}

        public int Line { get; set; }

        public override string ToString() 
        {
            return $"Message: {Message}, source: {Source}, line number: {Line}";
        }
    }
}
