using MVVM.HTML.Core.HTMLBinding;
using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding.Mapping
{
    public interface IJavascriptToCSharpConverter
    {
        IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject key, Type iTargetType);

        void RegisterInSession(object nv, Action<IJSCSGlue> Continue);
    }
}
