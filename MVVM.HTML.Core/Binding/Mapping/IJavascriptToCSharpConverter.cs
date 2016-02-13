using MVVM.HTML.Core.HTMLBinding;
using System;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;
using System.Threading.Tasks;

namespace MVVM.HTML.Core.Binding.Mapping
{
    public interface IJavascriptToCSharpConverter
    {
        IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject key, Type iTargetType);

        Task<IJSCSGlue> RegisterInSession(object nv);
    }
}
