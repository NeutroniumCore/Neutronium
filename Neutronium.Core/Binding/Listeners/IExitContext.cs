using Neutronium.Core.Binding.GlueObject;
using Neutronium.Core.WebBrowserEngine.JavascriptObject;
using System;

namespace Neutronium.Core.Binding.Listeners
{
    internal interface IExitContext : IDisposable
    {
        void AddToUnlisten(IJavascriptObject exiting);

        void SetBridgeUpdater(BridgeUpdater bridgeUpdater);
    }
}