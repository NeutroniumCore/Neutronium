//using System;
//using System.Threading.Tasks;
//using MVVM.HTML.Core.JavascriptEngine.Window;

//namespace MVVM.Cef.Glue.Test.CefWindowless
//{
//    internal class TestIUIDispatcher : IDispatcher
//    {
//        public Task RunAsync(Action act)
//        {
//            act();
//            return Task.FromResult<object>(null);
//        }

//        public void Run(Action act)
//        {
//            act();
//        }

//        public Task<T> EvaluateAsync<T>(Func<T> compute)
//        {
//            return Task.FromResult(compute());
//        }

//        public T Evaluate<T>(Func<T> compute)
//        {
//            return compute();
//        }
//    }
//}
