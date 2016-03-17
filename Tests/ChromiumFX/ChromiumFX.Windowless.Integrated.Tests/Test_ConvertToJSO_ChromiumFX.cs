using ChromiumFX.TestInfra;
using IntegratedTest.Tests.Windowless;
using Xunit;

namespace ChromiumFX.Windowless.Integrated.Tests 
{
    [Collection("ChromiumFX Windowless Integrated")]
    public class Test_ConvertToJSO_ChromiumFX : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_ChromiumFX(ChromiumFXWindowLessHTMLEngineProvider context):  base(context)
        {
        }
    }
}
