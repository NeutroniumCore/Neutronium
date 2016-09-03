using Ko.Binding.ChromiumFx.Tests.Infra;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Ko.Binding.ChromiumFx.Tests
{
    [Collection("Cfx Ko Windowless Integrated")]
    public class Binding_Ko_Cfx_Tests : HTMLBindingTests
    {
        public Binding_Ko_Cfx_Tests(CfxKoContext context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
