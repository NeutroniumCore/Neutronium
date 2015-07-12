using CefGlue.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.CEFGlue.Test.CefWindowless
{
    internal class TestIUIDispatcher : IUIDispatcher
    {
        public Task RunAsync(Action act)
        {
            act();
            return Task.FromResult<object>(null);
        }
    }
}
