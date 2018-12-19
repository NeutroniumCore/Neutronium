using Chromium;
using Neutronium.Core;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        public PackUriSchemeHandlerFactory(IWebSessionLogger webSessionLogger)
        {
            Create+= (s, e) =>
            {
                var handler = LocalUriResourceHandler.FromPackUrl(e.Request, webSessionLogger);
                e.SetReturnValue(handler);
            };
        }
    }
}