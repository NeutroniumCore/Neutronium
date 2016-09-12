using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.JavascriptEngine.JavascriptObject;

namespace Neutronium.Core.Binding
{
    public interface IJavascriptToCSharpConverter
    {
        IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject key, Type iTargetType);

        Task<IJSCSGlue> RegisterInSession(object nv);
    }
}
