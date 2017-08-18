using System;
using Chromium;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        public PackUriSchemeHandlerFactory()
        {
            Create+= (s, e) =>
            {
                e.SetReturnValue(new PackUriResourceHandler(new Uri(e.Request.Url)));
            };
        }
    }
}