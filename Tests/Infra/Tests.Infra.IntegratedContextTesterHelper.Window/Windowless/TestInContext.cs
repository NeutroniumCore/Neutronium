using System;
using Neutronium.Core;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public class TestInContext : TestContextBase
    {
        public Action<IHTMLBinding> Test { get; set; }
    }
}
