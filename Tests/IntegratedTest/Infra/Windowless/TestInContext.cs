using System;
using MVVM.HTML.Core;

namespace IntegratedTest.Infra.Windowless
{
    public class TestInContext : TestContextBase
    {
        public Action<IHTMLBinding> Test { get; set; }
    }
}
