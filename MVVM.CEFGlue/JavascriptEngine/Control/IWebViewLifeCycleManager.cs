using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.Navigation
{
    public interface IWebViewLifeCycleManager
    {
        IHTMLWindowProvider Create();

        IDispatcher GetDisplayDispatcher();

        //void Display(Control webview);

        //void Dispose(Control ioldwebview);
    }
}
