using Chromium;
using System;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    internal struct ChromiumFxCRemoteContext : IDisposable
    {
        private readonly CfxRemoteCallContext _CfxRemoteCallContext;

        public ChromiumFxCRemoteContext(CfxRemoteCallContext context)
        {
            _CfxRemoteCallContext = context;
            _CfxRemoteCallContext.Enter();
        }

        public void Dispose()
        {
            _CfxRemoteCallContext.Exit();
        }
    }
}
