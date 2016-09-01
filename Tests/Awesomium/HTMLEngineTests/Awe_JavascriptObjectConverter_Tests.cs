using Tests.Awesomium.HTMLEngineTests.Infra;
using Tests.Universal.HTMLEngineTests;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Awesomium.HTMLEngineTests
{
    [Collection("Awesomium Context")]
    public class Awe_JavascriptObjectConverter_Tests : JavascriptObjectConverter_Tests
    {
        public Awe_JavascriptObjectConverter_Tests(AwesomiumContext testEnvironment, ITestOutputHelper output)
                        : base(testEnvironment, output)
        {
        }
    }
}
