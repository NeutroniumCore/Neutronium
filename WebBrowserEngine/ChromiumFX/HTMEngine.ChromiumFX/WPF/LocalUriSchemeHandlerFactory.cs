using Chromium;
using Neutronium.Core;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class LocalUriSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        public LocalUriSchemeHandlerFactory(IWebSessionLogger webSessionLogger)
        {
            Create+= (s, e) =>
            {
                var handler = LocalUriResourceHandler.FromLocalUrl(e.Request, webSessionLogger);
                e.SetReturnValue(handler);
            };
        }
    }
}