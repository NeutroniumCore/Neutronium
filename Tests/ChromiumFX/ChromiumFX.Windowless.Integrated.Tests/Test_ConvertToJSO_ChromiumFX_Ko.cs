using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace ChromiumFX.Windowless.Integrated.Tests
{
    [Collection("ChromiumFX Ko Windowless Integrated")]
    public class Test_ConvertToJSO_ChromiumFX_Ko : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_ChromiumFX_Ko(ChromiumFXWindowLessHTMLEngineProviderKo context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
