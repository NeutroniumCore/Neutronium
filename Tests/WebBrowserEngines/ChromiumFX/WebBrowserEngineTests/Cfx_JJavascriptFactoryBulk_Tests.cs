using Tests.ChromiumFX.WebBrowserEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.ChromiumFX.WebBrowserEngineTests
{
    [Collection("ChromiumFX Context")]
    public class Cfx_JavascriptFactoryBulk_Tests : JavascriptFactoryBulk_Tests
    {
        public Cfx_JavascriptFactoryBulk_Tests(ChromiumFXContext testEnvironment, ITestOutputHelper output)
                        : base(testEnvironment, output)
        {
        }

        protected override bool SupportStringEmpty => true;
    }
}
