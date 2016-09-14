using Xunit;

namespace Tests.Awesomium.WebBrowserEngineTests.Infra
{
    [CollectionDefinition("Awesomium Context")]
    public class AwesomiumContextFixture : ICollectionFixture<AwesomiumContext> 
    {
    }
}