using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using Neutronium.Core.WebBrowserEngine.Window;

namespace Tests.Infra.HTMLEngineTesterHelper.Threading 
{
    public class DispatcherContextTaskScheduler :  TaskScheduler 
    {
        private readonly IDispatcher _Dispatcher;
        public DispatcherContextTaskScheduler(IDispatcher dispatcher) 
        {
            _Dispatcher = dispatcher;
        }

        [SecurityCritical]
        protected override void QueueTask(Task task) 
        {
            _Dispatcher.RunAsync(() => TryExecuteTask(task));
        }

        [SecurityCritical]
        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued) 
        {
            return (_Dispatcher.IsInContext() && TryExecuteTask(task));
        }

        [SecurityCritical]
        protected override IEnumerable<Task> GetScheduledTasks() 
        {
            return null;
        }

        public override Int32 MaximumConcurrencyLevel 
        {
            get { return 1; }
        }
    }
}
