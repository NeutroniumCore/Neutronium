using System;
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
                e.SetReturnValue(new PackUriResourceHandler(new Uri(e.Request.Url), webSessionLogger));
            };
        }
    }
}