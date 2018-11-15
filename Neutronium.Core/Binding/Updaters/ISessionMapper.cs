using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;
using System.Threading.Tasks;

namespace Neutronium.Core.Binding.Updaters
{
    internal interface ISessionMapper
    {
        Task<IJavascriptObject> InjectInHtmlSession(IJsCsGlue root);

        event EventHandler<EventArgs> OnJavascriptSessionReady;
    }
}
