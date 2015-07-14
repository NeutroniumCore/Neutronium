using CefGlue.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Xilium.CefGlue.WPF
{
    public class WPFUIDispatcher : IUIDispatcher
    {
        private Dispatcher _Dispatcher;
        public WPFUIDispatcher(Dispatcher iDispatcher)
        {
            _Dispatcher = iDispatcher;
        }

        public Task RunAsync(Action act)
        {
            var tcs = new TaskCompletionSource<object>();
            Action doact =()=>
                {
                    act();
                    tcs.SetResult(null);
                };
            _Dispatcher.BeginInvoke(doact);
            return tcs.Task;
        }


        public void Run(Action act)
        {
            _Dispatcher.Invoke(act);
        }
    }
}
