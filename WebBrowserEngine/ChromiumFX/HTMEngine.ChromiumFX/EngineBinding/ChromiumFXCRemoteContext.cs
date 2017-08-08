using System;
using Chromium;
using Chromium.Remote;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding 
{
    internal struct ChromiumFxCRemoteContext : IDisposable 
    {
        private readonly CfxRemoteCallContext _CfxRemoteCallContext;
        internal bool IsInContext { get; }

        public ChromiumFxCRemoteContext(CfrBrowser browser) 
        {
            IsInContext = CfxRemoteCallContext.IsInContext;
            _CfxRemoteCallContext = IsInContext ? null : browser.CreateRemoteCallContext();
            _CfxRemoteCallContext?.Enter();
        }

        public void Dispose() 
        {
            _CfxRemoteCallContext?.Exit();
        }
    }
}
