using System;
using Chromium;
using Chromium.Remote;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding 
{
    internal struct ChromiumFxCRemoteContext : IDisposable 
    {
        private readonly CfxRemoteCallContext _CfxRemoteCallContext;

        public ChromiumFxCRemoteContext(CfrBrowser browser) 
        {
            _CfxRemoteCallContext = (CfxRemoteCallContext.IsInContext) ? null : browser.CreateRemoteCallContext();
            _CfxRemoteCallContext?.Enter();
        }

        public void Dispose() 
        {
            _CfxRemoteCallContext?.Exit();
        }
    }
}
