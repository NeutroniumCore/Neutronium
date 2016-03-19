using System;
using Chromium.Remote;
using Chromium.Remote.Event;

namespace ChromiumFX.TestInfra.Helper 
{
    public class RenderProcessHandler : CfrRenderProcessHandler 
    {
        internal RenderProcessHandler()
        {
            this.OnContextCreated += RenderProcessHandler_OnContextCreated;
            this.OnBrowserCreated += RenderProcessHandler_OnBrowserCreated;
        }

        private void RenderProcessHandler_OnBrowserCreated(object sender, CfrOnBrowserCreatedEventArgs e) 
        {
            if (OnNewBrowser != null)
                OnNewBrowser(e);
        }

        private void RenderProcessHandler_OnContextCreated(object sender, CfrOnContextCreatedEventArgs e)
        {
            if (OnNewFrame != null)
                OnNewFrame(e);
        }

        public event Action<CfrOnBrowserCreatedEventArgs> OnNewBrowser;

        public event Action<CfrOnContextCreatedEventArgs> OnNewFrame;
    }
}
