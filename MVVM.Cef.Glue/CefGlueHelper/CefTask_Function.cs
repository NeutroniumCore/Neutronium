using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace MVVM.Cef.Glue.CefGlueHelper
{
    internal class FunctionTask<T> : CefTask
    {
        private Func<T> _Function;
        private TaskCompletionSource<T> _TaskCompletionSource;
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
