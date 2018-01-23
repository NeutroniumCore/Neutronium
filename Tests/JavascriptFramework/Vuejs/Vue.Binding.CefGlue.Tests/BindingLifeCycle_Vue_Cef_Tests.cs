using Tests.Universal.HTMLBindingTests;
using Vue.Binding.CefGlue.Tests.Infra;
using Xunit;
using Xunit.Abstractions;

namespace Vue.Binding.CefGlue.Tests
{
    [Collection("Cef Vue Windowless Integrated")]
    public class Binding_Vue_Cef_Tests : HtmlBindingTests
    {
        public Binding_Vue_Cef_Tests(CefGlueVueContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
