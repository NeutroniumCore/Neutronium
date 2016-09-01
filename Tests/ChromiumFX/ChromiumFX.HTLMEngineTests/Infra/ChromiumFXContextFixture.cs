using Xunit;

namespace Tests.ChromiumFX.HTLMEngineTests.Infra
{
    [CollectionDefinition("ChromiumFX Context")]
    public class ChromiumFXContextFixture : ICollectionFixture<ChromiumFXContext> 
    {
    }
}