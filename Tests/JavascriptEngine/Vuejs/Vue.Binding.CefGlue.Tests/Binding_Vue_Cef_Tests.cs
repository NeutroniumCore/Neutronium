using Vue.Binding.CefGlue.Tests.Infra;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Vue.Binding.CefGlue.Tests
{
    [Collection("Cef Vue Windowless Integrated")]
    public class Binding_Vue_Cef_Tests : HTMLBindingTests
    {
        public Binding_Vue_Cef_Tests(CefGlueVueContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
