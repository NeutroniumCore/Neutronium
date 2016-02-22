using MVVM.HTML.Core.JavascriptEngine.Window;
using System;

namespace MVVM.HTML.Core.Window
{
    public interface IHTMLModernWindow : IHTMLWindow
    {
        event EventHandler<BeforeJavascriptExcecutionArgs> BeforeJavascriptExecuted;
    }
}
