using System;
using System.Threading.Tasks;
using Neutronium.Core.Binding;
using Tests.Infra.WebBrowserEngineTesterHelper.HtmlContext;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public class TestContextBase<TContext> where TContext: IDisposable
    {
        public TestContext Path { get; set; }

        public Func<HtmlViewEngine, Task<TContext>> Bind { get; set; }
    }
}
