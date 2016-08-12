using System;
using System.Threading.Tasks;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Binding;
using UIFrameworkTesterHelper;

namespace IntegratedTest.Infra.Windowless
{
    public class TestInContextAsync
    {
        public TestContext Path { get; set; }

        public Func<HTMLViewEngine, Task<IHTMLBinding>> Bind { get; set; }

        public Func<IHTMLBinding, Task> Test { get; set; }
    }
}
