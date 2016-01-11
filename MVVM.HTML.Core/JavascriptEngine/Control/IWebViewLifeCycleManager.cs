using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.Window;

namespace MVVM.HTML.Core.Navigation
{
    public interface IWebViewLifeCycleManager
    {
        IHTMLWindowProvider Create();

        IDispatcher GetDisplayDispatcher();
    }
}
