using System;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Navigation
{
    public class FirstLoadEvent : EventArgs
    {
        private IWebBrowserWindow Window { get; }

        public FirstLoadEvent(IWebBrowserWindow window)
        {
            Window = window;
        }
    }
}
