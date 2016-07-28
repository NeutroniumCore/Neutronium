using ChromiumFX.TestInfra;
using Xunit;

namespace ChromiumFX.Window.Integrated.Tests
{
    [CollectionDefinition("ChromiumFX Window Integrated")]
    public class ChromiumFXWindowTestContext : ICollectionFixture<ChromiumFXWindowKoTestEnvironment> 
    {
    }
}
