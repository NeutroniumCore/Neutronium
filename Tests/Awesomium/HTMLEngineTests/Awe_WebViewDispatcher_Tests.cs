using Tests.Awesomium.HTMLEngineTests.Infra;
using Tests.Universal.WebBrowserEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Awesomium.HTMLEngineTests
{
    [Collection("Awesomium Context")]
    public class Awe_WebViewDispatcher_Tests : WebViewDispatcher_Tests
    {
        public Awe_WebViewDispatcher_Tests(AwesomiumContext testEnvironment, ITestOutputHelper output)
            : base(testEnvironment, output)
        {
        }
    }
}
