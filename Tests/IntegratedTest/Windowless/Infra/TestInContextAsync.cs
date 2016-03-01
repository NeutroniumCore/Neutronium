using System;
using System.Threading.Tasks;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Binding;

namespace IntegratedTest
{
    public class TestInContextAsync
    {
        public string Path { get; set; }

        public Func<HTMLViewEngine, Task<IHTMLBinding>> Bind { get; set; }

        public Func<IHTMLBinding, Task> Test { get; set; }
    }
}
