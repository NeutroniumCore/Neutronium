using System.Threading;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace MVVM.HTML.Core.Infra
{
    internal class DispatcherSynchronizationContext: SynchronizationContext
    {
        private readonly IDispatcher _Dispatcher;
        internal DispatcherSynchronizationContext(IDispatcher dispatcher) 
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
            _Dispatcher.Run(()=>d(state));
        }

        public override void Send(SendOrPostCallback d, object state)
        {
            _Dispatcher.RunAsync(() => d(state));
        }
    }
}
