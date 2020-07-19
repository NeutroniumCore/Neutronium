using System;
using System.Threading.Tasks;
using Neutronium.Core;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public class TestInContextAsync<TContext> : TestContextBase<TContext> where TContext : IDisposable
    {
        public Func<TContext, Task> Test { get; set; }
    }

    public class TestInContextAsync : TestInContextAsync<IHtmlBinding>
    {
    }
}
