using System;
using System.Threading.Tasks;
using MVVM.HTML.Core.Binding.GlueObject;
using MVVM.HTML.Core.JavascriptEngine.JavascriptObject;

namespace MVVM.HTML.Core.Binding
{
    public interface IJavascriptToCSharpConverter
    {
        IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject key, Type iTargetType);

        Task<IJSCSGlue> RegisterInSession(object nv);
    }
}
