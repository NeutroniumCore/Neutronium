using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;
using System;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.Binding.Mapping
{
    interface IJavascriptSessionInjector : IDisposable
    {
        IJavascriptObject Inject(IJavascriptObject ihybridobject, IJavascriptMapper ijvm, bool checknullvalue = true);
        Task RegisterMainViewModel(IJavascriptObject iJSObject);
    }
}
