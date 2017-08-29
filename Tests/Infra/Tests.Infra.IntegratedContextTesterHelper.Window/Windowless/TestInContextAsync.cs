using System;
using System.Threading.Tasks;
using Neutronium.Core;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public class TestInContextAsync : TestContextBase
    {
        public Func<IHtmlBinding, Task> Test { get; set; }
    }
}
