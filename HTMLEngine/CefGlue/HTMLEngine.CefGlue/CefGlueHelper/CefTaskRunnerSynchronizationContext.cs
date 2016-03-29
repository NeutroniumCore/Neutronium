using System.Threading;
using Xilium.CefGlue;

namespace HTMLEngine.CefGlue.CefGlueHelper
{
    internal class CefTaskRunnerSynchronizationContext : SynchronizationContext
    {
        private readonly CefTaskRunner _TaskRunner;

        internal CefTaskRunnerSynchronizationContext(CefTaskRunner cefTaskRunner)
        {
            _TaskRunner = cefTaskRunner;
            SetWaitNotificationRequired();
        }
        public override SynchronizationContext CreateCopy()
        {
            return new CefTaskRunnerSynchronizationContext(_TaskRunner);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            _TaskRunner.DispatchAsync(() => d(state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            _TaskRunner.RunAsync(() => d(state)). Wait();
        }
    }
}
