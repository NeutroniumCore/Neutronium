using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;
using System;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.Binding.Mapping
{
    internal interface IJavascriptSessionInjector : IDisposable
    {
        IJavascriptObject Inject(IJavascriptObject rawObject, IJavascriptMapper ijvm, bool checknullvalue = true);
        Task RegisterMainViewModel(IJavascriptObject rawObject);
    }
}
