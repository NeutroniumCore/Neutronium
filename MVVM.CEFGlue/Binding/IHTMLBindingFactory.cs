using System;
using System.Threading.Tasks;
using Xilium.CefGlue;
using Xilium.CefGlue.WPF;

namespace MVVM.CEFGlue
{
    public interface IHTMLBindingFactory
    {
        Task<IHTMLBinding> Bind(WpfCefBrowser view, object iViewModel, JavascriptBindingMode iMode);

        Task<IHTMLBinding> Bind(WpfCefBrowser view, object iViewModel, object addinfo, JavascriptBindingMode iMode);

        Task<IHTMLBinding> Bind(WpfCefBrowser view, string json);
    }
}
