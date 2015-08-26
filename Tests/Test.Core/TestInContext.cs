using MVVM.HTML.Core;
using MVVM.HTML.Core.JavascriptEngine;
using MVVM.HTML.Core.Window;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Cef.Glue.Test
{
    public class TestInContext
    {
        public TestInContext()
        {
        }

        public string Path { get; set; }

        public Func<IHTMLWindowProvider, Task<IHTMLBinding>> Bind { get; set; }

        public Action<IHTMLBinding> Test { get; set; }

        public Action<IHTMLBinding> Then { get; set; }
    }
}
