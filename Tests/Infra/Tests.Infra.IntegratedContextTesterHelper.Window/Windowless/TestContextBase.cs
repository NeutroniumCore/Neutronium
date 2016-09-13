using System;
using System.Threading.Tasks;
using Neutronium.Core;
using Neutronium.Core.Binding;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public class TestContextBase
    {
        public TestContext Path { get; set; }

        public Func<HTMLViewEngine, Task<IHTMLBinding>> Bind { get; set; }
    }
}
