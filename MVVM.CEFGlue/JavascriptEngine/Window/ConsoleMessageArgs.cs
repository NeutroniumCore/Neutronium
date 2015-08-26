using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.JavascriptEngine
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
    }
}
