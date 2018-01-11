using System;

namespace Neutronium.Core.WebBrowserEngine.Window
{
    public class ClientReloadArgs: EventArgs
    {
        public string Url { get; }

        public ClientReloadArgs(string url)
        {
            Url = url;
        }
    }
}
