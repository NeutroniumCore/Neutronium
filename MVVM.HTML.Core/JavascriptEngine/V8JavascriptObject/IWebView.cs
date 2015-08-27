using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MVVM.HTML.Core.Window;


namespace MVVM.HTML.Core.V8JavascriptObject
{
    public interface IWebView : IDispatcher
    {
        Task DispatchAsync(Action act);

        IJavascriptObject GetGlobal();

        IJavascriptObjectConverter Converter { get; }

        IJavascriptObjectFactory Factory { get; }

        bool Eval(string code, out IJavascriptObject res);

        void ExecuteJavaScript(string code);
    }
}
