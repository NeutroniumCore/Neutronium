using Chromium.Remote;
using System;

namespace HTMEngine.ChromiumFX.EngineBinding
{
    public class ChromiumFXTask
    {
        public ChromiumFXTask(Action perform)
        {
            var task = new CfrTask();
            task.Execute += (sender, args) =>
            {
                perform();
                Clean?.Invoke();
            };
            Task = task;
        }

        public CfrTask Task { get;  }

        public Action Clean { get; set; }
    }
}
