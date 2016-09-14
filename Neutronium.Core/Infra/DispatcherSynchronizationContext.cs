using System.Threading;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Neutronium.Core.Infra
{
    public class DispatcherSynchronizationContext: SynchronizationContext
    {
        private readonly IDispatcher _Dispatcher;
        public DispatcherSynchronizationContext(IDispatcher dispatcher) 
        {
            _Dispatcher = dispatcher;
            SetWaitNotificationRequired();
        }

        public override SynchronizationContext CreateCopy()
        {
            return new DispatcherSynchronizationContext(_Dispatcher);
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            _Dispatcher.RunAsync(()=>d(state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            _Dispatcher.Run(() => d(state));
        }
    }
}
