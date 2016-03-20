using ChromiumFX.TestInfra;
using IntegratedTest.Tests.Windowless;
using Xunit;

namespace ChromiumFX.Windowless.Integrated.Tests 
{
    [Collection("ChromiumFX Windowless Integrated")]
    public class Test_HTMLBinding_ChromiumFX : Test_HTMLBinding
    {
        public Test_HTMLBinding_ChromiumFX(ChromiumFXWindowLessHTMLEngineProvider context):  base(context)
        {
        }
    }
}
