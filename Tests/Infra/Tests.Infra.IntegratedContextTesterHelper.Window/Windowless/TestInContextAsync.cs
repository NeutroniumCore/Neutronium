using System;
using System.Threading.Tasks;
using MVVM.HTML.Core;

namespace Tests.Infra.IntegratedContextTesterHelper.Windowless
{
    public class TestInContextAsync : TestContextBase
    {
        public Func<IHTMLBinding, Task> Test { get; set; }
    }
}
