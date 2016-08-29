using IntegratedTest.Tests.Windowless;
using Xunit;
using Xunit.Abstractions;

namespace ChromiumFX.Windowless.Integrated.Tests
{
    [Collection("ChromiumFX Vue Windowless Integrated")]
    public class Test_HTMLBinding_ChromiumFX_Vue : Test_HTMLBinding
    {
        public Test_HTMLBinding_ChromiumFX_Vue(ChromiumFXWindowLessHTMLEngineProviderVue context, ITestOutputHelper output)
            : base(context, output)
        {
            context.Logger = _Logger;
        }
    }
}
