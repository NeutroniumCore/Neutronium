using System;
using Chromium;
using Chromium.Remote;

namespace HTMEngine.ChromiumFX.EngineBinding 
{
    internal class ChromiumFXCRemoteContext : IDisposable 
    {
        private readonly CfxRemoteCallContext _CfxRemoteCallContext;

        public ChromiumFXCRemoteContext(CfrBrowser browser) 
        {
            _CfxRemoteCallContext = (CfxRemoteCallContext.IsInContext) ? null : browser.CreateRemoteCallContext();
            if (_CfxRemoteCallContext!=null)
                _CfxRemoteCallContext.Enter();
        }

        public void Dispose() 
        {
            if (_CfxRemoteCallContext != null)
                _CfxRemoteCallContext.Exit();
        }
    }
}
