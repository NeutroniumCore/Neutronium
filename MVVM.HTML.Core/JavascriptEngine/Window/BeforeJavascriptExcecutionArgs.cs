using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.JavascriptEngine.Window
{
    public class BeforeJavascriptExcecutionArgs : EventArgs
    {
        public BeforeJavascriptExcecutionArgs(Action<string> javascriptExecutor)
        {
            JavascriptExecutor = javascriptExecutor;
        }

        public Action<string> JavascriptExecutor { get; private set; }
    }
}
