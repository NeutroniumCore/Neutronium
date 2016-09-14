using Xunit;

namespace Tests.ChromiumFX.WebBrowserEngineTests.Infra
{
    [CollectionDefinition("ChromiumFX Context")]
    public class ChromiumFXContextFixture : ICollectionFixture<ChromiumFXContext> 
    {
    }
}