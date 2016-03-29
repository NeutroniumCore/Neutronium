using System;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace HTMLEngine.CefGlue.CefGlueHelper
{
    internal class FunctionTask<T> : CefTask
    {
        private readonly Func<T> _Function;
        private readonly TaskCompletionSource<T> _TaskCompletionSource;
        public FunctionTask(Func<T> iFunction)
        {
            _Function = iFunction;
            _TaskCompletionSource = new TaskCompletionSource<T>();
        }

        internal Task<T> Task
        {
            get { return _TaskCompletionSource.Task; }
        }

        protected override void Execute()
        {
            try
            {
                _TaskCompletionSource.TrySetResult(_Function());
            }
            catch (Exception e)
            {
                _TaskCompletionSource.TrySetException(e);
            }  
        }
    }
}
