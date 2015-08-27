using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core.V8JavascriptObject;
using MVVM.HTML.Core.JavascriptEngine;


namespace MVVM.HTML.Core.Window
{
    public interface IHTMLWindow
    {
        IWebView MainFrame { get; }

        void NavigateTo(string path);

        bool IsLoaded { get; }

        event EventHandler<LoadEndEventArgs> LoadEnd;

        event EventHandler<ConsoleMessageArgs> ConsoleMessage;

        IDispatcher GetDispatcher();
    }
}
