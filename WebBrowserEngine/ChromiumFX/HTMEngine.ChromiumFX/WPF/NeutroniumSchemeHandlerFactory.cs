using Chromium;
using Neutronium.Core;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class NeutroniumSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        public NeutroniumSchemeHandlerFactory(IWebSessionLogger webSessionLogger)
        {
            Create += (s, e) =>
            {
                var handler = NeutroniumResourceHandler.FromHttpsUrl(e.Request, webSessionLogger);
                e.SetReturnValue(handler);
            };
        }
    }
}