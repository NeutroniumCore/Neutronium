using System;
using Neutronium.Core;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public class TestInContext<TContext> : TestContextBase<TContext> where TContext : IDisposable
    {
        public Action<TContext> Test { get; set; }
    } 

    public class TestInContext : TestInContext<IHtmlBinding>
    {
    }
}
