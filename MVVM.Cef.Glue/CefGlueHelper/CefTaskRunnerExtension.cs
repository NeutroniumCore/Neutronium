using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.CefGlueHelper
{
    public static class CefTaskRunnerExtension
    {
        public static Task RunAsync(this CefTaskRunner @this, Action actionToRun)
        {
            if (@this.BelongsToCurrentThread)
            {
                Trace.TraceInformation("CefTaskRunnerExtension.RunAsync: called from same Thread: no switch");
                actionToRun();
                return Task.FromResult<object>(null);
            }

            Trace.TraceInformation("CefTaskRunnerExtension.RunAsync: called from different Thread: switching");
            var actiontask = new CefTask_Action(actionToRun);
            @this.PostTask(actiontask);
            return actiontask.Task;
        }

        public static Task DispatchAsync(this CefTaskRunner @this, Action actionToRun)
        {
            var actiontask = new CefTask_Action(actionToRun);
            @this.PostTask(actiontask);
            return actiontask.Task;
        }

        public static Task<T> EvaluateAsync<T>(this CefTaskRunner @this, Func<T> evaluate)
        {
            if (@this.BelongsToCurrentThread)
            {
                Trace.TraceInformation("CefTaskRunnerExtension.EvaluateAsync: called from same Thread: no switch");
                return Task.FromResult(evaluate());
            }

            Trace.TraceInformation("CefTaskRunnerExtension.EvaluateAsync: called from different Thread: switching");
            var functionTask = new FunctionTask<T>(evaluate);
            @this.PostTask(functionTask);
            return functionTask.Task;
        }
    }
}
