using CefGlue.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.CEFGlue.Test
{
    public class TestInContext
    {
        public TestInContext()
        {
        }

        public string Path { get; set; }

        public Func<ICefGlueWindow,Task<IHTMLBinding>> Bind { get; set; }

        public Action<IHTMLBinding> Test { get; set; }

    }
}
