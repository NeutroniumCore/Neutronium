using System;
using Chromium.Remote;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    public class ChromiumFxTask
    {
        public ChromiumFxTask(Action perform)
        {
            Task = new CfrTask();
            Task.Execute += (sender, args) =>
            {
                perform();
                Clean?.Invoke();
                Task.Dispose();
                Task = null;
            };
        }

        public CfrTask Task { get; private set; }

        public Action Clean { get; set; }
    }
}
