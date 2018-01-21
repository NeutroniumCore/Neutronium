using Mobx.CefGlue.Test.Infra;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Mobx.CefGlue.Test
{
    [Collection("Cef Mobx Windowless Integrated")]
    public class Binding_Mobx_Cef_Tests : HtmlBindingTests
    {
        public Binding_Mobx_Cef_Tests(CefGlueMobxContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
