using Ko.Binding.Awesomium.Tests.Infra;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Ko.Binding.Awesomium.Tests
{
    [Collection("Awesomium Ko Windowless Integrated")]
    public class Binding_Ko_Awe_Tests : HtmlBindingTests
    {
        public Binding_Ko_Awe_Tests(AwesomiumKoContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
