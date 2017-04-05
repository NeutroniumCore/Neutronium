using System;
using Chromium.Remote;

namespace Neutronium.WebBrowserEngine.ChromiumFx.EngineBinding
{
    public class ChromiumFxTask
    {
        public ChromiumFxTask(Action perform)
        {
            var task = new CfrTask();
            task.Execute += (sender, args) =>
            {
                perform();
                Clean?.Invoke();
                task.Dispose();
            };
            Task = task;
        }

        public CfrTask Task { get;  }

        public Action Clean { get; set; }
    }
}
