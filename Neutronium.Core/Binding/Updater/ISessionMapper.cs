using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;

namespace Neutronium.Core.Binding.Updater
{
    internal interface ISessionMapper
    {
        Task<IJavascriptObject> InjectInHtmlSession(IJsCsGlue root);

        event EventHandler<EventArgs> OnJavascriptSessionReady;
    }
}
