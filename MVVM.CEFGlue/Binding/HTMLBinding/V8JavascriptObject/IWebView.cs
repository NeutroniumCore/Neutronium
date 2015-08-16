using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xilium.CefGlue;

using CefGlue.Window;

namespace MVVM.CEFGlue.Binding.HTMLBinding.V8JavascriptObject
{
    public interface IWebView : IDispatcher
    {
        Task DispatchAsync(Action act);

        IJavascriptObject GetGlobal();

        IJavascriptObjectConverter Converter { get; }

        IJavascriptObjectFactory Factory { get; }

        bool Eval(string code, out IJavascriptObject res);
    }
}
