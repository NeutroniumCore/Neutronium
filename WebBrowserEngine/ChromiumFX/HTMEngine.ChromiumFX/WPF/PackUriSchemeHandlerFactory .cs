using Chromium;
using Neutronium.Core;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        private bool _First = true;
        public PackUriSchemeHandlerFactory(IWebSessionLogger webSessionLogger)
        {
            Create += (s, e) =>
            {
                if (_First)
                    webSessionLogger.Warning(@"'pack://' url are being deprecated due to some limitation when using routing, use 'local://' instead");
                _First = false;
                var handler = LocalUriResourceHandler.FromPackUrl(e.Request, webSessionLogger);
                e.SetReturnValue(handler);
            };
        }
    }
}