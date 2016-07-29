using ChromiumFX.TestInfra;
using Xunit;

namespace ChromiumFX.Window.Integrated.Tests
{
    [CollectionDefinition("ChromiumFX Window Vue Integrated")]
    public class ChromiumFXWindowVueTestContext : ICollectionFixture<ChromiumFXWindowVueTestEnvironment> 
    {
    }
}
