using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;
using MVVM.HTML.Core.JavascriptEngine.Window;

namespace IntegratedTest.Infra.Threading 
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
            return false;
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

        private void PostCallback(object obj) 
        {
            base.TryExecuteTask((Task) obj);
        }
    }
}
