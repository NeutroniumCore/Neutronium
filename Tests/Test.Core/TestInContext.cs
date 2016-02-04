using MVVM.HTML.Core;
using MVVM.HTML.Core.Binding;
using System;
using System.Threading.Tasks;

namespace MVVM.Cef.Glue.Test
{
    public class TestInContext
    {
        public TestInContext()
        {
        }

        public string Path { get; set; }

        public Func<HTMLViewEngine, Task<IHTMLBinding>> Bind { get; set; }

        public Action<IHTMLBinding> Test { get; set; }

        public Action<IHTMLBinding> Then { get; set; }
    }
}
