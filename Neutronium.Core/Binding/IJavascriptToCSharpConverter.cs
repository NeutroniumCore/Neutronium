using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding
{
    public interface IJavascriptToCSharpConverter
    {
        IJSCSGlue GetCachedOrCreateBasic(IJavascriptObject key, Type targetType);

        Task RegisterInSession(object nv, Action<IJSCSGlue> performAfterBuild);
    }
}
