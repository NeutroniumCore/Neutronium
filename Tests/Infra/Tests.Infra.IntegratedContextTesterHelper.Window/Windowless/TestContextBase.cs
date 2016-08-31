using System;
using System.Threading.Tasks;
using MVVM.HTML.Core;
using MVVM.HTML.Core.Binding;
using Tests.Infra.HTMLEngineTesterHelper.HtmlContext;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public class TestContextBase
    {
        public TestContext Path { get; set; }

        public Func<HTMLViewEngine, Task<IHTMLBinding>> Bind { get; set; }
    }
}
