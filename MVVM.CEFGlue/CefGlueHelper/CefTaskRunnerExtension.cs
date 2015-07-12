using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefGlueHelper
{
    public static class CefTaskRunnerExtension
    {
        public static Task RunInContextAsync(this CefTaskRunner @this, Action actionToRun)
        {
            if (@this.BelongsToCurrentThread)
            {
                actionToRun();
                return Task.FromResult<object>(null);
            }

            var actiontask = new CefTask_Action(actionToRun);
            @this.PostTask(actiontask);
            return actiontask.Task;
        }

        public static Task<T> EvaluateAsync<T>(this CefTaskRunner @this, Func<T> evaluate)
        {
            if (@this.BelongsToCurrentThread)
            {
                return Task.FromResult(evaluate());
            }

            var functionTask = new FunctionTask<T>(evaluate);
            @this.PostTask(functionTask);
            return functionTask.Task;
        }
    }
}
