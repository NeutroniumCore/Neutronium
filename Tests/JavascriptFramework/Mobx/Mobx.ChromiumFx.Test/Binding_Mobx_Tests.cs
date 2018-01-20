using Mobx.ChromiumFx.Test.Infra;
using Tests.Universal.HTMLBindingTests;
using Xunit;
using Xunit.Abstractions;

namespace Mobx.ChromiumFx.Test
{
    [Collection("Mobx Windowless Integrated")]
    public class Binding_Mobx_Tests : HtmlBindingTests
    {
        public Binding_Mobx_Tests(MobxVueContext testEnvironment, ITestOutputHelper output) :
            base(testEnvironment, output)
        {
        }
    }
}
