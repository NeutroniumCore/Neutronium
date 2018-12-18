using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Chromium;
using Neutronium.Core;

namespace Neutronium.WebBrowserEngine.ChromiumFx.WPF
{
    public class PackUriSchemeHandlerFactory : CfxSchemeHandlerFactory
    {
        private readonly List<PackUriResourceHandler> _PackUriResourceHandlers = new List<PackUriResourceHandler>();
        private static readonly Regex _PackUrl = new Regex(@"^pack:\/\/", RegexOptions.Compiled);
        private static readonly Regex _OldPackUrl = new Regex(@"^pack:\/\/application:,,,\/", RegexOptions.Compiled);

        private const string Prefix = @"pack://application:,,,/";

        public PackUriSchemeHandlerFactory(IWebSessionLogger webSessionLogger)
        {
            Create+= (s, e) =>
            {
                var handler = new PackUriResourceHandler(UpdateUrl(e.Request.Url), webSessionLogger);
                _PackUriResourceHandlers.Add(handler);
                e.SetReturnValue(handler);
            };
        }

        internal static string UpdateLoadUrl(string url)
        {
           return _OldPackUrl.Replace(url, "pack://");
        }

        internal static Uri UpdateUrl(string url)
        {
            var newUrl = _PackUrl.Replace(url, Prefix);
            return new Uri(newUrl);
        }
    }
}