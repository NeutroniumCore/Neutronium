using ChromiumFX.TestInfra;
using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace ChromiumFX.Windowless.Integrated.Tests 
{
    [Collection("ChromiumFX Windowless Integrated")]
    public class Test_HTMLBinding_ChromiumFX : Test_HTMLBinding
    {
        public Test_HTMLBinding_ChromiumFX(ChromiumFXWindowLessHTMLEngineProvider context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
