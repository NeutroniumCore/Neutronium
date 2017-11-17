using System;
using System.Collections.Generic;
using Chromium;
using Neutronium.Core;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        private readonly List<PackUriResourceHandler> _PackUriResourceHandlers = new List<PackUriResourceHandler>();

        public PackUriSchemeHandlerFactory(IWebSessionLogger webSessionLogger)
        {
            Create+= (s, e) =>
            {
                var handler = new PackUriResourceHandler(new Uri(e.Request.Url), webSessionLogger);
                _PackUriResourceHandlers.Add(handler);
                e.SetReturnValue(handler);
            };
        }
    }
}