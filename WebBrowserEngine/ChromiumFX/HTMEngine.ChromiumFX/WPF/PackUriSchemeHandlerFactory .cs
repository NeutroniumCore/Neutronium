using Chromium;
using Neutronium.Core;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        private readonly IWebSessionLogger _WebSessionLogger;
        private bool _First = true;
        public PackUriSchemeHandlerFactory(IWebSessionLogger webSessionLogger)
        {
            _WebSessionLogger = webSessionLogger;
            Create += (s, e) =>
            {
                if (_First)
                    _WebSessionLogger.Error($"'pack:\\' url are beging deprecated due to some limitation when using routing, use 'local:\\' instead");
                _First = false;
                var handler = LocalUriResourceHandler.FromPackUrl(e.Request, webSessionLogger);
                e.SetReturnValue(handler);
            };
        }
    }
}