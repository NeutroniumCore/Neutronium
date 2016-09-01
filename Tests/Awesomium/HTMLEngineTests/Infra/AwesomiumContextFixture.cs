using Xunit;

namespace Tests.Awesomium.HTMLEngineTests.Infra
{
    [CollectionDefinition("Awesomium Context")]
    public class AwesomiumContextFixture : ICollectionFixture<AwesomiumContext> 
    {
    }
}