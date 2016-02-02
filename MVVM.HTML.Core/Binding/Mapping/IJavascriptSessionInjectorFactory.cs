using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding.Mapping;
using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;

namespace MVVM.HTML.Core.Binding
{
    public interface IJavascriptSessionInjectorFactory
    {
        IJavascriptSessionInjector CreateInjector(IWebView iWebView, IJavascriptChangesListener iJavascriptListener);
    }
}
