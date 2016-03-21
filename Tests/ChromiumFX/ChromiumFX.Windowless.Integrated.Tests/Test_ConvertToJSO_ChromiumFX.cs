using ChromiumFX.TestInfra;
using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace ChromiumFX.Windowless.Integrated.Tests 
{
    [Collection("ChromiumFX Windowless Integrated")]
    public class Test_ConvertToJSO_ChromiumFX : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_ChromiumFX(ChromiumFXWindowLessHTMLEngineProvider context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
