using System;
using System.Threading.Tasks;
using MVVM.HTML.Core;

namespace IntegratedTest.Infra.Windowless
{
    public class TestInContextAsync : TestContextBase
    {
        public Func<IHTMLBinding, Task> Test { get; set; }
    }
}
