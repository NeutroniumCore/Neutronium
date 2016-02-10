using MVVM.HTML.Core.HTMLBinding;
using MVVM.HTML.Core.V8JavascriptObject;
using System;

namespace MVVM.HTML.Core.Binding.Mapping
{
    public interface IJavascriptToCSharpConverter
    {
        IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject key, Type iTargetType);

        void RegisterInSession(object nv, Action<IJSCSGlue> Continue);
    }
}
