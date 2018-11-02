using System;
using System.Threading.Tasks;
using Xilium.CefGlue;

namespace Neutronium.WebBrowserEngine.CefGlue.CefGlueHelper
{
    internal class CefTask_Action : CefTask
    {
        private readonly Action _Action;
        private readonly TaskCompletionSource<object> _TaskCompletionSource;
        public CefTask_Action(Action iAction)
        {
            _Action = iAction;
            _TaskCompletionSource = new TaskCompletionSource<object>();
        }

        internal Task Task => _TaskCompletionSource.Task;

        protected override void Execute()
        {
            try
            {
                _Action();
            }
            catch (Exception e)
            {
                _TaskCompletionSource.TrySetException(e);
                return;
            }

            _TaskCompletionSource.TrySetResult(null);
        }
    }
}
