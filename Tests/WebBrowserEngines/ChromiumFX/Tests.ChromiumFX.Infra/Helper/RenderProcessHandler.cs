using System;
using Chromium.Remote;
using Chromium.Remote.Event;

namespace Tests.ChromiumFX.Infra.Helper 
{
    public class RenderProcessHandler : CfrRenderProcessHandler 
    {
        public CfrApp App { get; }
        public int ProcessId { get; }

        internal RenderProcessHandler(CfrApp app, int processId) 
        {
            App = app;
            ProcessId = processId;
            this.OnContextCreated += RenderProcessHandler_OnContextCreated;
            this.OnBrowserCreated += RenderProcessHandler_OnBrowserCreated;
        }

        private void RenderProcessHandler_OnBrowserCreated(object sender, CfrOnBrowserCreatedEventArgs e) 
        {
            OnNewBrowser?.Invoke(e);
        }

        private void RenderProcessHandler_OnContextCreated(object sender, CfrOnContextCreatedEventArgs e) 
        {
            OnNewFrame?.Invoke(e);
        }

        public event Action<CfrOnBrowserCreatedEventArgs> OnNewBrowser;

        public event Action<CfrOnContextCreatedEventArgs> OnNewFrame;
    }
}
