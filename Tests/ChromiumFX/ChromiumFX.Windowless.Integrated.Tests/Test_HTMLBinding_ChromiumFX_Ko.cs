using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace ChromiumFX.Windowless.Integrated.Tests
{
    [Collection("ChromiumFX Ko Windowless Integrated")]
    public class Test_HTMLBinding_ChromiumFX_Ko : Test_HTMLBinding
    {
        public Test_HTMLBinding_ChromiumFX_Ko(ChromiumFXWindowLessHTMLEngineProviderKo context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
