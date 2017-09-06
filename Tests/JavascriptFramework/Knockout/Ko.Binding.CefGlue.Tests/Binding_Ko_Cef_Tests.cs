using Ko.Binding.CefGlue.Tests.Infra;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Ko.Binding.CefGlue.Tests
{
    [Collection("Cef Ko Windowless Integrated")]
    public class Binding_Ko_Cef_Tests : HtmlBindingTests
    {
        public Binding_Ko_Cef_Tests(CefGlueKoContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
