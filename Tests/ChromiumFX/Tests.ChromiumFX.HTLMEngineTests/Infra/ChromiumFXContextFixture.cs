using Xunit;

namespace Tests.ChromiumFX.HTMLEngineTests.Infra
{
    [CollectionDefinition("ChromiumFX Context")]
    public class ChromiumFXContextFixture : ICollectionFixture<ChromiumFXContext> 
    {
    }
}