using Mobx.CefGlue.Test.Infra;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Mobx.CefGlue.Test
{
    [Collection("Cef Mobx Windowless Integrated")]
    public class BindingLifeCycle_Mobx_Cef_Tests : BindingLifeCycleTests
    {
        public BindingLifeCycle_Mobx_Cef_Tests(CefGlueMobxContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
