using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace ChromiumFX.Windowless.Integrated.Tests
{
    [Collection("ChromiumFX Vue Windowless Integrated")]
    public class Test_ConvertToJSO_ChromiumFX_Vue : Test_ConvertToJSO
    {
        public Test_ConvertToJSO_ChromiumFX_Vue(ChromiumFXWindowLessHTMLEngineProviderVue context, ITestOutputHelper output)
            : base(context, output)
        {
        }
    }
}
