using MVVM.CEFGlue.Infra;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.CEFGlue.CefGlueHelper
{
    public static class CefV8ContextExtension
    {

      
        public static Task RunInContextAsync(this CefV8Context @this, Action actionToRun)
        {
            var runner = @this.GetTaskRunner();
            return runner.RunInContextAsync(actionToRun);
        }

        public static Task CreateInContextAsync(this CefV8Context @this, Action actionToRun)
        {
            Action na = () =>
                {
                    @this.Enter();
                    actionToRun();
                    @this.Exit();
                };
            return @this.RunInContextAsync(na);
        }

        public static Task<T> EvaluateInCreateContextAsync<T>(this CefV8Context @this, Func<T> evaluate)
        {
            Func<T> na = () =>
            {
                @this.Enter();
                var res = evaluate();
                @this.Exit();
                return res;
            };
            return @this.EvaluateAsync(na);
        }



        public static Task<T> EvaluateAsync<T>(this CefV8Context @this, Func<T> evaluate)
        {
            var runner = @this.GetTaskRunner();
            return runner.EvaluateAsync<T>(evaluate);
        }
    }
}
