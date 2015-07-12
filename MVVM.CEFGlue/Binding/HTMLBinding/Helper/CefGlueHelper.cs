using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;
using CefGlue.Window;

namespace MVVM.CEFGlue.HTMLBinding
{
    public static class CefGlueHelper
    {
        //        //private static Task<T> EvaluateSafeAsync<T>(this CefV8Context iwb, Func<T> evaluate)
        //        //{
        //        //    TaskCompletionSource<T> tcs = new TaskCompletionSource<T>();
        //        //    WebCore.QueueWork(iwb, () => tcs.SetResult(evaluate()));
        //        //    return tcs.Task;
        //        //}

        //        //public static T EvaluateSafe<T>(this CefV8Context iwb, Func<T> evaluate)
        //        //{
        //        //    try
        //        //    {
        //        //        return evaluate();
        //        //    }
        //        //    catch (InvalidOperationException)
        //        //    {
        //        //        return iwb.EvaluateSafeAsync(evaluate).Result;
        //        //    }
        //        //}

        public static void ExecuteWhenReady(this ICefGlueWindow view, Action ToBeApply)
        {
            Action Check = () =>
                {
                    bool done = false;
                    EventHandler<LoadEndEventArgs> leeh = null;
                    leeh = (o, e) =>
                    {
                        ToBeApply();
                        done = true;
                        view.LoadEnd -= leeh;
                    };

                    view.LoadEnd += leeh;
                    if ((view.IsLoaded) && !done)
                        ToBeApply();
                };

            //view.Dispatcher.BeginInvoke(Check);
            view.GetDispatcher().RunAsync(Check);
        }
    }
}
