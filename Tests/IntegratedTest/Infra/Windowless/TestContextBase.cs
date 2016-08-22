using MVVM.HTML.Core;
using MVVM.HTML.Core.Binding;
using System;
using System.Threading.Tasks;
using UIFrameworkTesterHelper;

namespace IntegratedTest.Infra.Windowless
{
    public class TestContextBase
    {
        public TestContext Path { get; set; }

        public Func<HTMLViewEngine, Task<IHTMLBinding>> Bind { get; set; }
    }
}
